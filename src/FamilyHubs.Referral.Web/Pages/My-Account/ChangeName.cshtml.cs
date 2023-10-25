using System.Collections.Immutable;
using FamilyHubs.Referral.Core.ApiClients;
using FamilyHubs.Referral.Web.Pages.Shared;
using FamilyHubs.SharedKernel.Identity;
using FamilyHubs.SharedKernel.Razor.Errors;
using Microsoft.AspNetCore.Mvc;
using ErrorDictionary = System.Collections.Immutable.ImmutableDictionary<int, FamilyHubs.SharedKernel.Razor.Errors.Error>;

namespace FamilyHubs.Referral.Web.Pages.My_Account;

public class ChangeNameModel : HeaderPageModel
{
    public enum ErrorId
    {
        EnterAName
    }

    public static readonly ErrorDictionary AllErrors = ImmutableDictionary
        .Create<int, SharedKernel.Razor.Errors.Error>()
        .Add(ErrorId.EnterAName, "new-name", "Enter a name");

    private readonly IIdamsClient _idamsClient;

    public IErrorState ErrorState { get; private set; }

    [BindProperty]
    public string? FullName { get; set; }

    public ChangeNameModel(IIdamsClient idamsClient)
    {
        _idamsClient = idamsClient;

        ErrorState = SharedKernel.Razor.Errors.ErrorState.Empty;
    }

    public void OnGet()
    {
        FullName = HttpContext.GetFamilyHubsUser().FullName;
    }

    //todo: PRG?
    public async Task<IActionResult> OnPost(CancellationToken cancellationToken)
    {
        if (ModelState.IsValid && !string.IsNullOrWhiteSpace(FullName) && FullName.Length <= 255)
        {
            var familyHubsUser = HttpContext.GetFamilyHubsUser();

            //todo: common client with dto's in package
            var request = new UpdateAccountSelfServiceDto
            {
                AccountId = long.Parse(familyHubsUser.AccountId),
                Name = FullName
            };
            await _idamsClient.UpdateAccountSelfService(request, cancellationToken);

            HttpContext.RefreshClaims();

            return RedirectToPage("ChangeNameConfirmation");
        }

        //todo: overload/replace with params version
        ErrorState = SharedKernel.Razor.Errors.ErrorState.Create(AllErrors, new[] { ErrorId.EnterAName });

        return Page();
    }
}