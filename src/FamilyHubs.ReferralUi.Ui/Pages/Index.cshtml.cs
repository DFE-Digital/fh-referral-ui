using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.ReferralUi.Ui.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public bool IsReferralEnabled { get; set; } = false;

    public IndexModel(IConfiguration configuration, ILogger<IndexModel> logger)
    {
        _logger = logger;
        IsReferralEnabled = configuration.GetValue<bool>("IsReferralEnabled");
    }

    public void OnGet()
    {

    }
}
