using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class ConfirmationModel : PageModel
{
    public int RequestNumber { get; set; }

    public void OnGet(int requestNumber)
    {
        RequestNumber = requestNumber;
    }
}