using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral
{
    [Authorize(Policy = "Referrer")]
    public class ConsentShutterModel : PageModel
    {
        public void OnGet()
        {
            //Standard GET method for page
        }
    }
}
