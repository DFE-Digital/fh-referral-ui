using FamilyHubs.ReferralUi.Ui.Models;
using FamilyHubs.ReferralUi.Ui.Services;
using FamilyHubs.ReferralUi.Ui.Services.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;

[Authorize(Policy = "Referrer")]
public class ConnectFamilyToServiceStartModel : PageModel
{
    private readonly IRedisCacheService _redisCacheService;

    [BindProperty]
    public string Id { get; set; } = default!;
    [BindProperty]
    public string Name { get; set; } = default!;

    public ConnectFamilyToServiceStartModel(IRedisCacheService redisCacheService)
    {
        _redisCacheService = redisCacheService;
    }
    public void OnGet(string id, string name)
    {
        Id = id;
        Name = name;

        string userKey = _redisCacheService.GetUserKey();
        _redisCacheService.ResetConnectWizzardViewModel(userKey);
        ConnectWizzardViewModel model = _redisCacheService.RetrieveConnectWizzardViewModel(userKey);
        if (string.IsNullOrEmpty(model.ServiceId)) 
        {
            model = new ConnectWizzardViewModel
            {
                ServiceId = Id,
                ServiceName = Name,
                ReferralId = User?.Identity?.Name ?? userKey.Replace("ConnectWizzardViewModel-", "")
            };
        }

        _redisCacheService.StoreConnectWizzardViewModel(userKey, model);

    }

    public IActionResult OnPost(string id, string name)
    {
        return RedirectToPage("/ProfessionalReferral/Safeguarding", new
        {
        });
    }
}
