using FamilyHubs.Referral.Web.Pages.Shared;
using FamilyHubs.SharedKernel.Razor.FamilyHubsUi.Options;
using Microsoft.Extensions.Options;

namespace FamilyHubs.Referral.Web.Pages.Contact_Us;

public class IndexModel : HeaderPageModel
{
    public IFamilyHubsUiOptions FamilyHubsUiOptions { get; }

    public IndexModel(IOptions<FamilyHubsUiOptions> familyHubsUiOptions)
    {
        FamilyHubsUiOptions = familyHubsUiOptions.Value;
    }
}