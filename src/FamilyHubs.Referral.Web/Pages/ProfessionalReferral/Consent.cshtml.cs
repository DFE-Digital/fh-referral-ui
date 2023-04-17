using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class ConsentModel : PageModel
{ 
    [BindProperty]
    public string ServiceId { get; set; } = default!;
    [BindProperty]
    public string ServiceName { get; set; } = default!;

    [BindProperty]
    public string Consent { get; set; } = default!;


    [BindProperty]
    public bool ValidationValid { get; set; } = true;

    public void OnGet(string serviceId, string serviceName)
    {
        ServiceId = serviceId;
        ServiceName = serviceName;
    }

    public IActionResult OnPost(string serviceId, string serviceName)
    {
        if (!ModelState.IsValid || Consent == null)
        {
            ValidationValid = false;
            return Page();
        }


        if (string.Compare(Consent, "yes", StringComparison.OrdinalIgnoreCase) == 0)
        {
            

            return RedirectToPage("/ProfessionalReferral/SupportDetails", new
            {
                serviceId,
                serviceName
            });

        }

        return RedirectToPage("/ProfessionalReferral/ConsentShutter", new
        {
        });

    }
}
