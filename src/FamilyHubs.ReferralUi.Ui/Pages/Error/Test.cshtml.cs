using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.ReferralUi.Ui.Pages.Error
{
    public class TestModel : PageModel
    {
        public void OnGet()
        {
#pragma warning disable S112
            throw new Exception("Fault injected exception");
#pragma warning restore S112
        }
    }
}
