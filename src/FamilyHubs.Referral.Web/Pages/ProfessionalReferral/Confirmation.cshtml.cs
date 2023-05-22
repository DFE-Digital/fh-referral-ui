using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class ConfirmationModel : PageModel
{
    public string? RequestNumber { get; set; }

    public void OnGet(string? requestNumber)
    {
        RequestNumber = requestNumber;
    }
}