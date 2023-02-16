using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics.CodeAnalysis;

namespace FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral
{
    //No logic so no need for unit tests
    [ExcludeFromCodeCoverage]
    [Authorize(Policy = "Referrer")]
    public class ConsentShutterModel : PageModel
    {
    }
}
