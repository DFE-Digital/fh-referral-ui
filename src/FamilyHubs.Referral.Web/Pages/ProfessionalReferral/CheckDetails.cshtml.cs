using FamilyHubs.Referral.Core.ApiClients;
using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Pages.Shared;
using FamilyHubs.ReferralService.Shared.Dto;
using FamilyHubs.SharedKernel.Identity;
using Microsoft.AspNetCore.Authorization;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

[Authorize]
public class CheckDetailsModel : ProfessionalReferralSessionModel
{
    private readonly IOrganisationClientService _organisationClientService;
    private readonly IReferralClientService _referralClientService;
    public ConnectionRequestModel ConnectionRequestModel { get; private set; } = new ConnectionRequestModel();

    public string? BackUrl { get; set; }
    public CheckDetailsModel(IConnectionRequestDistributedCache connectionRequestCache, 
        IOrganisationClientService organisationClientService,
        IReferralClientService referralClientService)
        : base(connectionRequestCache)
    {
        _organisationClientService = organisationClientService;
        _referralClientService = referralClientService;
    }

    protected override void OnGetWithModel(ConnectionRequestModel model)
    {
        ConnectionRequestModel = model;
        SetPageProperties(model);
    }

    protected override string? OnPostWithModel(ConnectionRequestModel model)
    {
        
        if(!CreateOrUpdateReferral(model).Result)
        {
            return null;
        }

        //todo: use next page
        return "ConfirmReferral";
    }

    private async Task<bool> CreateOrUpdateReferral(ConnectionRequestModel model)
    {
        try
        {
            var user = HttpContext.GetFamilyHubsUser();

            ReferrerDto referrerDto = new ReferrerDto
            { 
                Name = $"{user.FirstName} {user.LastName}",
                EmailAddress = user.Email,
                Role = user.Role,
                PhoneNumber = user.PhoneNumber,
                Team = ""
                
            };


            ServiceDirectory.Shared.Dto.ServiceDto serviceDto = await _organisationClientService.GetLocalOfferById(model.ServiceId ?? "0");
            ServiceDirectory.Shared.Dto.OrganisationDto? organisation = await _organisationClientService.GetOrganisationDtobyIdAsync(serviceDto.OrganisationId);

            bool isNewReferral = true;
            ReferralDto dto = new ReferralDto
            {
                ReasonForSupport = model.Reason!,
                EngageWithFamily = model.EngageReason!,
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                RecipientDto = new RecipientDto
                {
                    Name = model.FamilyContactFullName!,
                    Email = model.EmailAddress!,
                    Telephone = model.TelephoneNumber!,
                    TextPhone = model.TextphoneNumber!,
                    AddressLine1 = model.AddressLine1!,
                    AddressLine2 = model.AddressLine2!,
                    TownOrCity = model.TownOrCity!,
                    Country = model.County!,
                    PostCode = model.Postcode
                },
                ReferrerDto = referrerDto,
                ReferralServiceDto = new ReferralServiceDto
                {
                    Id = serviceDto.Id,
                    Name = serviceDto.Name,
                    Description = serviceDto.Description,
                    ReferralOrganisationDto = new ReferralOrganisationDto
                    { 
                        Name = organisation?.Name!,
                        Description = organisation?.Description!,
                    }
                }
            };

            if (model.ReferralId != null)
            {
                ReferralDto? original = null;

                try
                {
                    original = await _referralClientService.GetReferralById(model.ReferralId.Value.ToString());
                }
                catch
                {
                    //Original can not be found so just continue and add a new one
                }

                if (original != null)
                {
                    isNewReferral = false;
                    dto.ReasonForSupport = model.Reason!;
                    dto.EngageWithFamily = model.EngageReason!;
                    dto.Created = DateTime.UtcNow;
                    dto.LastModified = DateTime.UtcNow;
                    dto.RecipientDto.Name = model.FamilyContactFullName!;
                    dto.RecipientDto.Email = model.EmailAddress;
                    dto.RecipientDto.Telephone = model.TelephoneNumber!;
                    dto.RecipientDto.TextPhone = model.TextphoneNumber!;
                    dto.RecipientDto.AddressLine1 = model.AddressLine1!;
                    dto.RecipientDto.AddressLine2 = model.AddressLine2!;
                    dto.RecipientDto.TownOrCity = model.TownOrCity!;
                    dto.RecipientDto.Country = model.County!;
                    dto.RecipientDto.PostCode = model.Postcode;

                    dto.ReferrerDto = referrerDto;
                    dto.ReferrerDto.Id = original.ReferrerDto.Id;
                    dto.ReferralServiceDto.Name = serviceDto.Name;
                    dto.ReferralServiceDto.Description = serviceDto.Description;
                    dto.ReferralServiceDto.ReferralOrganisationDto = new ReferralOrganisationDto
                    {
                        Id = serviceDto.OrganisationId,
                        Name = organisation?.Name!,
                        Description = organisation?.Description!,
                    };


                }
                
            }

            if (isNewReferral)
            {
                await _referralClientService.CreateReferral(dto);
            }
            else
            {
                await _referralClientService.UpdateReferral(dto);
            }

            return true;
        }
        catch 
        {
            return false;
        }
        
    }
    private void SetPageProperties(ConnectionRequestModel model)
    {
        BackUrl = PreviousPage(ConnectJourneyPage.ContactMethods, model.ContactMethodsSelected);
    }
}
