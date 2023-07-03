using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.SharedKernel.Identity;
using FamilyHubs.SharedKernel.Razor.FamilyHubsUi.Delegators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

[Authorize(Roles = RoleGroups.LaProfessionalOrDualRole)]
public class ConfirmationModel : PageModel, IFamilyHubsHeader
{
    private readonly IConnectionRequestDistributedCache _connectionRequestCache;

    public ConfirmationModel(IConnectionRequestDistributedCache connectionRequestCache)
    {
        _connectionRequestCache = connectionRequestCache;
    }

    public int RequestNumber { get; set; }

    public async Task OnGetAsync(int requestNumber)
    {
        var professionalUser = HttpContext.GetFamilyHubsUser();
        await _connectionRequestCache.RemoveAsync(professionalUser.Email);

        RequestNumber = requestNumber;
    }

    LinkStatus IFamilyHubsHeader.GetStatus(SharedKernel.Razor.FamilyHubsUi.Options.LinkOptions link)
    {
        return link.Text == "Search for service" ? LinkStatus.Active : LinkStatus.Visible;
    }
}