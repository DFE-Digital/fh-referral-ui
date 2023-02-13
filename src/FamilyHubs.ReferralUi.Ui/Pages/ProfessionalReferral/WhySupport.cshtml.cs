using FamilyHubs.ReferralUi.Ui.Models;
using FamilyHubs.ReferralUi.Ui.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;

[Authorize(Policy = "Referrer")]
public class WhySupportModel : PageModel
{
    private readonly IRedisCacheService _redisCacheService;

    [BindProperty]
    public string ReferralId { get; set; } = default!;
    [BindProperty]
    public string FullName { get; set; } = default!;

    [BindProperty]
    public string? Email { get; set; } = default!;

    [BindProperty]
    public string? Telephone { get; set; } = default!;

    [BindProperty]
    public string? Textphone { get; set; } = default!;

    [BindProperty]
    public string ReasonForSupport { get; set; } = default!;

    [BindProperty]
    public string Id { get; set; } = default!;
    [BindProperty]
    public string Name { get; set; } = default!;

    [BindProperty]
    public bool ValidationValid { get; set; } = true;

    public WhySupportModel(IRedisCacheService redisCacheService)
    {
        _redisCacheService = redisCacheService;
    }

    public void OnGet(string id, string name, string fullName, string email, string telephone, string textphone, string reasonForSupport, string referralId)
    {
        Id = id;
        Name = name;
        FullName = fullName;
        Email = email;
        Telephone = telephone;
        Textphone = textphone;
        ReferralId = referralId;

        string userKey = _redisCacheService.GetUserKey();
        ConnectWizzardViewModel model = _redisCacheService.RetrieveConnectWizzardViewModel(userKey);
        Id = model.ServiceId;
        Name = model.ServiceName;
        ReferralId = model.ReferralId;
        FullName = model.FullName;
        Email = model.EmailAddress;
        Telephone = model.Telephone;
        Textphone = model.Textphone;
        if (!string.IsNullOrEmpty(model.ReasonForSupport))
            ReasonForSupport = model.ReasonForSupport;

        if (!string.IsNullOrEmpty(reasonForSupport))
            ReasonForSupport = reasonForSupport;
    }

    public IActionResult OnPost()
    {
        ModelState.Remove("Email");
        ModelState.Remove("Telephone");
        ModelState.Remove("Textphone");
        ModelState.Remove("ReferralId");

        if (ReasonForSupport == null || ReasonForSupport.Trim().Length == 0 || ReasonForSupport.Length > 500)
        {
            ValidationValid = false;
            return Page();
        }

        string userKey = _redisCacheService.GetUserKey();
        ConnectWizzardViewModel model = _redisCacheService.RetrieveConnectWizzardViewModel(userKey);
        model.ReasonForSupport = ReasonForSupport;
        _redisCacheService.StoreConnectWizzardViewModel(userKey, model);

        return RedirectToPage("/ProfessionalReferral/CheckReferralDetails", new
        {
            id = Id,
            name = Name,
            fullName = FullName,
            email = Email,
            telephone = Telephone,
            textphone = Textphone,
            reasonForSupport = ReasonForSupport,
            referralId = ReferralId
        });

    }
}
