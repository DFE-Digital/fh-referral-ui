using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.ReferralUi.Ui.Pages;

public class IndexModel : PageModel
{
    public bool IsReferralEnabled { get; set; } = false;

    public IndexModel(IConfiguration configuration, ILogger<IndexModel> logger)
    {
        IsReferralEnabled = configuration.GetValue<bool>("IsReferralEnabled");
    }
}
