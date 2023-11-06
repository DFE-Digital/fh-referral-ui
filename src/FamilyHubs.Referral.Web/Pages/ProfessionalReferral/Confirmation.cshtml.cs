using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.SharedKernel.Identity;
using FamilyHubs.SharedKernel.Razor.FamilyHubsUi.Options;
using FamilyHubs.SharedKernel.Razor.Header;
using FamilyHubs.SharedKernel.Razor.Links;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.IO;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

[Authorize(Roles = RoleGroups.LaProfessionalOrDualRole)]
public class ConfirmationModel : PageModel, IFamilyHubsHeader
{
    private readonly IConnectionRequestDistributedCache _connectionRequestCache;
    private readonly IOptions<FamilyHubsUiOptions> _familyHubsUiOptions;

    public string? ConnectionRequestUrl { get; private set; }

    public ConfirmationModel(IConnectionRequestDistributedCache connectionRequestCache, IOptions<FamilyHubsUiOptions> familyHubOptions)
    {
        _connectionRequestCache = connectionRequestCache;
        _familyHubsUiOptions = familyHubOptions;
        
    }

    public int RequestNumber { get; set; }

    public async Task OnGetAsync(int requestNumber)
    {
        var professionalUser = HttpContext.GetFamilyHubsUser();
        await _connectionRequestCache.RemoveAsync(professionalUser.Email);
        ConnectionRequestUrl = _familyHubsUiOptions.Value.Url(UrlKeys.DashboardWeb, $"la/RequestDetails?id={requestNumber}").ToString();

        RequestNumber = requestNumber;
    }

    LinkStatus IFamilyHubsHeader.GetStatus(IFhRenderLink link)
    {
        return link.Text == "Search for service" ? LinkStatus.Active : LinkStatus.Visible;
    }
}