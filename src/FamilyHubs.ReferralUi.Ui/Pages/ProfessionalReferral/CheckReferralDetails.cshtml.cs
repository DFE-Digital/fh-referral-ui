using FamilyHubs.ReferralUi.Ui.Models;
using FamilyHubs.ReferralUi.Ui.Services;
using FamilyHubs.ReferralUi.Ui.Services.Api;
using FamilyHubs.ServiceDirectory.Shared.Dto;
using FamilyHubs.ServiceDirectory.Shared.MassTransit;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;

[Authorize(Policy = "Referrer")]
public class CheckReferralDetailsModel : PageModel
{
    private readonly IRedisCacheService _redisCacheService;

    [BindProperty]
    public string ReferralId { get; set; } = default!;

    [BindProperty]
    public string FullName { get; set; } = default!;

    [BindProperty]
    public string? Email { get; set; } = default!;

    [BindProperty]
    public string? Telephone { get; set; } = default!;

    [BindProperty]
    public string? Textphone { get; set; } = default!;

    [BindProperty]
    public string ReasonForSupport { get; set; } = default!;

    [BindProperty]
    public string Id { get; set; } = default!;
    [BindProperty]
    public string Name { get; set; } = default!;

    private readonly IConfiguration _configuration;
    private readonly ILocalOfferClientService _localOfferClientService;
    private readonly IReferralClientService _referralClientService;

    public CheckReferralDetailsModel(IConfiguration configuration, ILocalOfferClientService localOfferClientService, IReferralClientService referralClientService, IRedisCacheService redisCacheService)
    {
        _referralClientService = referralClientService;
        _configuration = configuration;
        _localOfferClientService = localOfferClientService;
        _redisCacheService = redisCacheService;
    }

    public void OnGet()
    {
        string userKey = _redisCacheService.GetUserKey();
        ConnectWizzardViewModel model = _redisCacheService.RetrieveConnectWizzardViewModel(userKey);

        Id = model.ServiceId;
        Name = model.ServiceName;
        ReferralId = model.ReferralId;
        FullName = model.FullName;
        Email = model.EmailAddress;
        Telephone = model.Telephone;
        Textphone = model.Textphone;
        ReasonForSupport = model.ReasonForSupport;
    }

    public async Task<IActionResult> OnPost()
    {
        // Save to API
        string userKey = _redisCacheService.GetUserKey();
        ConnectWizzardViewModel model = _redisCacheService.RetrieveConnectWizzardViewModel(userKey);

        ServiceDto serviceDto = await _localOfferClientService.GetLocalOfferById(model.ServiceId);

        try
        {
            bool isNewReferral = true;
            ReferralDto dto;
            if (string.IsNullOrEmpty(model.ReferralId))
            {
                dto = new(Guid.NewGuid().ToString(), serviceDto.OrganisationId, serviceDto.Id, serviceDto.Name, serviceDto.Description ?? String.Empty, Newtonsoft.Json.JsonConvert.SerializeObject(serviceDto), model.ReferralId, model.FullName, string.Empty, model.EmailAddress, model.Telephone, model.Textphone, model.ReasonForSupport, null, new List<ReferralStatusDto> { new ReferralStatusDto(Guid.NewGuid().ToString(), "Initial Connection") });
            }
            else
            {
                ReferralDto? original = null;
                    
                try
                {
                    original = await _referralClientService.GetReferralById(ReferralId);
                }
                catch
                {
                    //Original can not be found so just continue and add a new one
                }
                    
                if (original != null) 
                {
                    isNewReferral = false;
                    dto = new(model.ReferralId, serviceDto.OrganisationId, serviceDto.Id, serviceDto.Name, serviceDto.Description ?? String.Empty, Newtonsoft.Json.JsonConvert.SerializeObject(serviceDto), model.ReferralId, model.FullName, string.Empty, model.EmailAddress, model.Telephone, model.Textphone, model.ReasonForSupport, null, original.Status);
                }
                else
                {
                    dto = new(Guid.NewGuid().ToString(), serviceDto.OrganisationId, serviceDto.Id, serviceDto.Name, serviceDto.Description ?? String.Empty, Newtonsoft.Json.JsonConvert.SerializeObject(serviceDto), model.ReferralId, model.FullName, string.Empty, model.EmailAddress, model.Telephone, model.Textphone, model.ReasonForSupport, null, new List<ReferralStatusDto> { new ReferralStatusDto(Guid.NewGuid().ToString(), "Initial Connection") });
                } 
            }

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
                if (isNewReferral)
                {
                    await _referralClientService.CreateReferral(dto);
                }
                else
                {
                    await _referralClientService.UpdateReferral(dto);
                }
                
            }

            _redisCacheService.ResetConnectWizzardViewModel(userKey);
        }
        catch
        {
            return Page();
        }
        
        return RedirectToPage("/ProfessionalReferral/ConfirmReferral", new
        {
        });
    }
}
