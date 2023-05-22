using System.Security.Claims;
using FamilyHubs.Referral.Core.ApiClients;
using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Pages.Shared;
using FamilyHubs.ServiceDirectory.Shared.Dto;
using FamilyHubs.ReferralService.Shared.Dto;
using FamilyHubs.SharedKernel.Identity;
using FamilyHubs.SharedKernel.Identity.Models;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class CheckDetailsModel : ProfessionalReferralSessionModel
{
    private readonly IOrganisationClientService _organisationClientService;
    private readonly IReferralClientService _referralClientService;
    public ConnectionRequestModel? ConnectionRequestModel { get; set; }

    public CheckDetailsModel(
        IConnectionRequestDistributedCache connectionRequestCache,
        IOrganisationClientService organisationClientService,
        IReferralClientService referralClientService)
        : base(ConnectJourneyPage.CheckDetails, connectionRequestCache)
    {
        _organisationClientService = organisationClientService;
        _referralClientService = referralClientService;
    }

    protected override void OnGetWithModel(ConnectionRequestModel model)
    {
        // do this now, so we don't display any previously entered contact details that are no longer selected
        // but don't remove them from the session yet, in case the user goes back to change the contact details
        // we'll remove them from the session when the user submits the form
        RemoveNonSelectedContactDetails(model);

        ConnectionRequestModel = model;
    }

    private static void RemoveNonSelectedContactDetails(ConnectionRequestModel model)
    {
        // remove any previously entered contact details that are no longer selected
        //todo: can we do this generically?
        if (!model.ContactMethodsSelected[(int) ConnectContactDetailsJourneyPage.Email])
        {
            model.EmailAddress = null;
        }

        if (!model.ContactMethodsSelected[(int) ConnectContactDetailsJourneyPage.Telephone])
        {
            model.TelephoneNumber = null;
        }

        if (!model.ContactMethodsSelected[(int) ConnectContactDetailsJourneyPage.Textphone])
        {
            model.TextphoneNumber = null;
        }

        if (!model.ContactMethodsSelected[(int) ConnectContactDetailsJourneyPage.Letter])
        {
            model.AddressLine1 = null;
            model.AddressLine2 = null;
            model.TownOrCity = null;
            model.County = null;
            model.Postcode = null;
        }
    }

    protected override async Task<IActionResult> OnPostWithModelNew(ConnectionRequestModel model)
    {
        RemoveNonSelectedContactDetails(model);

        await CreateConnectionRequest(model);

        return NextPage("Confirmation");
    }

    private async Task CreateConnectionRequest(ConnectionRequestModel model)
    {
        //todo: this throws an ArgumentNullException is the service is not found. it should return null (from a 404 from the api)
        ServiceDto service = await _organisationClientService.GetLocalOfferById(model.ServiceId!);
        OrganisationDto? organisation = await _organisationClientService.GetOrganisationDtobyIdAsync(service.OrganisationId);

        if (organisation == null)
        {
            //todo: create and throw custom exception
            throw new InvalidOperationException($"Organisation not found for service {service.Id}");
        }   

        var user = HttpContext.GetFamilyHubsUser();
        var team = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "Team");

        var referralDto = CreateReferralDto(model, user, team, service, organisation);

        await _referralClientService.CreateReferral(referralDto);
    }

    private static ReferralDto CreateReferralDto(
        ConnectionRequestModel model,
        FamilyHubsUser user,
        Claim? team,
        ServiceDto service,
        OrganisationDto organisation)
    {
        var referralDto = new ReferralDto
        {
            ReasonForSupport = model.Reason!,
            EngageWithFamily = model.EngageReason!,
            RecipientDto = new RecipientDto
            {
                Name = model.FamilyContactFullName!,
                Email = model.EmailAddress,
                Telephone = model.TelephoneNumber,
                TextPhone = model.TextphoneNumber,
                AddressLine1 = model.AddressLine1,
                AddressLine2 = model.AddressLine2,
                TownOrCity = model.TownOrCity,
                County = model.County,
                PostCode = model.Postcode
            },
            ReferrerDto = new ReferrerDto
            {
                EmailAddress = user.Email,
                Name = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
                Team = team?.Value
            },
            ReferralServiceDto = new ReferralServiceDto
            {
                Id = service.Id,
                Name = service.Name,
                Description = service.Description,
                ReferralOrganisationDto = new ReferralOrganisationDto
                {
                    Id = organisation.Id,
                    Name = organisation.Name,
                    Description = organisation.Description
                }
            },
            Status = new ReferralStatusDto
            {
                Name = "New"
            },
            Created = DateTime.UtcNow
        };
        referralDto.LastModified = referralDto.Created;
        return referralDto;
    }
}