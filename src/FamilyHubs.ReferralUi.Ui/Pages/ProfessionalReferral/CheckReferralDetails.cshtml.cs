using FamilyHubs.ReferralUi.Ui.Models;
using FamilyHubs.ReferralUi.Ui.Services;
using FamilyHubs.ReferralUi.Ui.Services.Api;
using FamilyHubs.ServiceDirectory.Shared.Dto;
using FamilyHubs.ServiceDirectory.Shared.MassTransit;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;
using System.Text.Encodings.Web;
using MassTransit.Internals.GraphValidation;
using System.Net.Mail;
using EnumsNET;

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

    [BindProperty]
    public string OrganisationEmail { get; set; } = default!;
    public bool ValidationValid { get; set; } = true;

    private readonly IConfiguration _configuration;
    private readonly ILocalOfferClientService _localOfferClientService;
    private readonly IReferralClientService _referralClientService;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<CheckReferralDetailsModel> _logger;

    public CheckReferralDetailsModel(IConfiguration configuration, ILocalOfferClientService localOfferClientService, IReferralClientService referralClientService, IRedisCacheService redisCacheService, IEmailSender emailSender, ILogger<CheckReferralDetailsModel> logger)
    {
        _referralClientService = referralClientService;
        _configuration = configuration;
        _localOfferClientService = localOfferClientService;
        _redisCacheService = redisCacheService;
        _emailSender = emailSender;
        _logger = logger;
    }

    private void InitPage(ConnectWizzardViewModel model)
    {
        Id = model.ServiceId;
        Name = model.ServiceName;
        ReferralId = model.ReferralId;
        FullName = model.FullName;
        Email = model.EmailAddress;
        Telephone = model.Telephone;
        Textphone = model.Textphone;
        ReasonForSupport = model.ReasonForSupport;
    }

    public async Task OnGet()
    {
        string userKey = _redisCacheService.GetUserKey();
        ConnectWizzardViewModel model = _redisCacheService.RetrieveConnectWizzardViewModel(userKey);
        InitPage(model);
        ServiceDto serviceDto = await _localOfferClientService.GetLocalOfferById(model.ServiceId);
        PopulateOrganisationEmail(serviceDto);
    }

    private void PopulateOrganisationEmail(ServiceDto serviceDto)
    {
        
        if (serviceDto != null)
        {
            if (GetDeliveryMethodsAsString(serviceDto.ServiceDeliveries).Contains("In Person"))
                OrganisationEmail = serviceDto?.ServiceAtLocations?.ElementAt(0)?.LinkContacts?.ElementAt(0)?.Contact?.Email!;
            else
            {
                var contact = serviceDto?.LinkContacts?.Select(linkcontact => linkcontact.Contact).FirstOrDefault();
                if (contact != null)
                {
                    OrganisationEmail = contact.Email ?? default!; 
                }
            }
        }
    }

    private string GetDeliveryMethodsAsString(ICollection<ServiceDeliveryDto>? serviceDeliveries)
    {
        var result = string.Empty;

        if (serviceDeliveries == null || serviceDeliveries.Count == 0)
            return result;

        foreach (var name in serviceDeliveries.Select(serviceDelivery => serviceDelivery.Name))
        {
            result += result +
                    name.AsString(EnumFormat.Description) != null ?
                    name.AsString(EnumFormat.Description) + "," :
                    String.Empty;
        }

        //Remove last comma if present
        if (result.EndsWith(","))
        {
            result = result.Remove(result.Length - 1);
        }

        return result;
    }

    public async Task<IActionResult> OnPost()
    {
        // Save to API
        string[] ignore = { "Id", "Name", "FullName", "ReferralId", "ReasonForSupport" };
        foreach(string item in ignore) 
        {
            ModelState.Remove(item);
        }

        string userKey = _redisCacheService.GetUserKey();
        ConnectWizzardViewModel model = _redisCacheService.RetrieveConnectWizzardViewModel(userKey);

        ServiceDto serviceDto = await _localOfferClientService.GetLocalOfferById(model.ServiceId);

        try
        {
            bool isNewReferral = true;
            ReferralDto dto;
            if (string.IsNullOrEmpty(model.ReferralId))
            {
                dto = new(Guid.NewGuid().ToString(), serviceDto.OrganisationId, serviceDto.Id, serviceDto.Name, serviceDto.Description ?? String.Empty, Newtonsoft.Json.JsonConvert.SerializeObject(serviceDto), model.ReferralId, model.FullName, string.Empty, model.EmailAddress, model.Telephone, model.Textphone, DateTime.UtcNow,0L, model.ReasonForSupport, null, new List<ReferralStatusDto> { new ReferralStatusDto(Guid.NewGuid().ToString(), "Initial Connection") });
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
                    dto = new(model.ReferralId, serviceDto.OrganisationId, serviceDto.Id, serviceDto.Name, serviceDto.Description ?? String.Empty, Newtonsoft.Json.JsonConvert.SerializeObject(serviceDto), model.ReferralId, model.FullName, string.Empty, model.EmailAddress, model.Telephone, model.Textphone, DateTime.UtcNow, 0L, model.ReasonForSupport, null, original.Status);
                }
                else
                {
                    dto = new(Guid.NewGuid().ToString(), serviceDto.OrganisationId, serviceDto.Id, serviceDto.Name, serviceDto.Description ?? String.Empty, Newtonsoft.Json.JsonConvert.SerializeObject(serviceDto), model.ReferralId, model.FullName, string.Empty, model.EmailAddress, model.Telephone, model.Textphone, DateTime.UtcNow, 0L, model.ReasonForSupport, null, new List<ReferralStatusDto> { new ReferralStatusDto(Guid.NewGuid().ToString(), "Initial Connection") });
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

                model.ReferralId = dto.Id;
                _redisCacheService.StoreConnectWizzardViewModel(userKey,model);


            }

            if (!string.IsNullOrEmpty(OrganisationEmail))
            {
                await SendEmail(OrganisationEmail);
            }

            ValidationValid = ModelState.IsValid;
            if (!ModelState.IsValid) 
            {
                InitPage(model);
                return Page();
            }

            _redisCacheService.ResetConnectWizzardViewModel(userKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to complete CheckReferralDetailsModel.OnPost");
            InitPage(model);
            return Page();
        }
        
        return RedirectToPage("/ProfessionalReferral/ConfirmReferral", new
        {
        });
    }

    private async Task SendEmail(string emailAddress)
    {
        try
        {
            var callbackUrl = Url.Page(
                "/ProfessionalReferral/SignIn",
            pageHandler: null,
                values: new { },
                protocol: Request.Scheme);

            await _emailSender.SendEmailAsync(
                emailAddress,
                "New Connection",
                $"You have received a new connection, please logon to view it here: {HtmlEncoder.Default.Encode(callbackUrl ?? string.Empty)}");
        }
        catch(Exception ex) 
        {
            _logger.LogError(ex, "Failed to send email via Gov Notify");
            ModelState.AddModelError("OrganisationEmail", "Failed to send email via Gov Notify");
        }
        
    }
}
