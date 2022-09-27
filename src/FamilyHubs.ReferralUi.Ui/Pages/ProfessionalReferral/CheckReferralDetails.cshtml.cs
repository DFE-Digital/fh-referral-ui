using FamilyHubs.ReferralUi.Ui.Services.Api;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OpenReferralServices;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.Referrals;
using FamilyHubs.ServiceDirectory.Shared.Models.MassTransit;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;

public class CheckReferralDetailsModel : PageModel
{
    [BindProperty]
    public string FullName { get; set; } = default!;

    [BindProperty]
    public string HasSpecialNeeds { get; set; } = default!;

    [BindProperty]
    public string Email { get; set; } = default!;

    [BindProperty]
    public string Phone { get; set; } = default!;

    [BindProperty]
    public string ReasonForSupport { get; set; } = default!;

    [BindProperty]
    public string Id { get; set; } = default!;
    [BindProperty]
    public string Name { get; set; } = default!;

    private readonly IConfiguration _configuration;
    private readonly ILocalOfferClientService _localOfferClientService;
    private readonly IReferralClientService _referralClientService;
    public CheckReferralDetailsModel(IConfiguration configuration, ILocalOfferClientService localOfferClientService, IReferralClientService referralClientService)
    {
        _referralClientService = referralClientService;
        _configuration = configuration;
        _localOfferClientService = localOfferClientService;
    }

    public void OnGet(string id, string name, string fullName, string hasSpecialNeeds, string email, string phone, string reasonForSupport)
    {
        Id = id;
        Name = name;
        FullName = fullName;
        HasSpecialNeeds = hasSpecialNeeds;
        Email = email;
        Phone = phone;
        ReasonForSupport = reasonForSupport;
    }

    public async Task<IActionResult> OnPost()
    {
        // Save to API
        OpenReferralServiceDto openReferralServiceDto = await _localOfferClientService.GetLocalOfferById(Id);

        ReferralDto dto = new(Guid.NewGuid().ToString(),Id, openReferralServiceDto.OpenReferralOrganisationId, Name, openReferralServiceDto.Description  ?? String.Empty, Newtonsoft.Json.JsonConvert.SerializeObject(openReferralServiceDto), "CurrentUser",FullName,HasSpecialNeeds,Email,Phone,ReasonForSupport, new List<ReferralStatusDto> { new ReferralStatusDto(Guid.NewGuid().ToString(), "Initial-Referral") });

        try
        {
            if (_configuration.GetValue<bool>("UseRabbitMQ"))
            {
                using (var scope = Program.ServiceProvider.CreateScope())
                {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                    IPublishEndpoint publishEndPoint = scope.ServiceProvider.GetService<IPublishEndpoint>();
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                    if (publishEndPoint != null)
                        await publishEndPoint.Publish(new CommandMessage(Guid.NewGuid().ToString(), Newtonsoft.Json.JsonConvert.SerializeObject(dto)));
                }

            }
            else
            {
                await _referralClientService.CreateReferral(dto);
            }
        }
        catch
        {
            return Page();
        }
        

        return RedirectToPage("/ProfessionalReferral/ConfirmReferral", new
        {
            id = Id,
            name = Name,
            fullName = FullName,
            hasSpecialNeeds = HasSpecialNeeds,
            email = Email,
            phone = Phone,
            reasonForSupport = ReasonForSupport
        });
    }
}
