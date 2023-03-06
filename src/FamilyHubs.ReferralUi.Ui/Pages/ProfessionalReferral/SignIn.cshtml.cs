using FamilyHubs.ReferralUi.Ui.Services.Api;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;

public class SignInModel : PageModel
{
    private readonly IAuthService _authenticationService;
    private readonly ITokenService _tokenService;

    [BindProperty]
    public string Email { get; set; } = string.Empty;
    [BindProperty]
    public string Password { get; set; } = string.Empty;

    public bool ValidationValid { get; set; } = true;

    public SignInModel(IAuthService authenticationService, ITokenService tokenService)
    {
        _authenticationService = authenticationService;
        _tokenService = tokenService;
    }

    public async Task<IActionResult> OnPost()
    {
        Guid organisationId = new Guid("72e653e8-1d05-4821-84e9-9177571a6013");

        try
        {
            var tokenModel = await _authenticationService.Login(Email, Password);
            if (tokenModel != null)
            {

                var handler = new JwtSecurityTokenHandler();
                var jwtSecurityToken = handler.ReadJwtToken(tokenModel.Token);
                var claims = jwtSecurityToken.Claims.ToList();

                var claim = claims.FirstOrDefault(x => x.Type == "OpenReferralOrganisationId");
                if (claim != null)
                {
                    organisationId = new Guid(claim.Value);
                }

                var appIdentity = new ClaimsIdentity(claims);
                User.AddIdentity(appIdentity);

                //Initialize a new instance of the ClaimsIdentity with the claims and authentication scheme    
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                //Initialize a new instance of the ClaimsPrincipal with ClaimsIdentity    
                var principal = new ClaimsPrincipal(identity);

                _tokenService.SetToken(tokenModel.Token, jwtSecurityToken.ValidTo, tokenModel.RefreshToken);

                //SignInAsync is a Extension method for Sign in a principal for the specified scheme.    
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties()
                {
                    IsPersistent = false //Input.RememberMe,
                });
            }
        }
        catch (Exception)
        {
            ValidationValid = false;
            ModelState.AddModelError("Login", "Username or password is invalid");
            return Page();
        }

        if (User != null && User.IsInRole("Professional")) 
        {
            return RedirectToPage("/ProfessionalReferral/ProfessionalHomepage", new
            {
            });
        }

        if (User != null && User.IsInRole("VCSAdmin"))
        {
            return RedirectToPage("/ProfessionalReferral/ReferralDashboard", new
            {
                organisationId = organisationId
            });
        }

        return RedirectToPage("/ProfessionalReferral/Search", new
        {
        });
    }
}
