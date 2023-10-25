using FamilyHubs.Referral.Web.Pages.Shared;
using FamilyHubs.SharedKernel.Identity;

namespace FamilyHubs.Referral.Web.Pages.My_Account;

public class ChangeNameConfirmationModel : HeaderPageModel
{
    public string? NewName { get; set; }

    public void OnGet()
    {
        NewName = HttpContext.GetFamilyHubsUser().FullName;
    }
}