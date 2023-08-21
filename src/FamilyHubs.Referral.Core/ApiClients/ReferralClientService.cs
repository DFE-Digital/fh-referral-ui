﻿using System.Net.Http.Json;
using FamilyHubs.ReferralService.Shared.Dto;
using FamilyHubs.SharedKernel.Security;
using FamilyHubs.ReferralService.Shared.Models;

namespace FamilyHubs.Referral.Core.ApiClients;

public interface IReferralClientService
{
    Task<ReferralResponse> CreateReferral(ReferralDto referralDto, CancellationToken cancellationToken = default);
}

//todo: have single combined client (in referralshared)?
public class ReferralClientService : ApiService, IReferralClientService
{
    private readonly ICrypto _crypto;

    public ReferralClientService(HttpClient client, ICrypto crypto) : base(client)
    {
        _crypto = crypto;
    }

    public async Task<ReferralResponse> CreateReferral(ReferralDto referralDto, CancellationToken cancellationToken = default)
    {
        referralDto.ReasonForSupport = await _crypto.EncryptData(referralDto.ReasonForSupport);
        referralDto.EngageWithFamily = await _crypto.EncryptData(referralDto.EngageWithFamily);

        using var response = await Client.PostAsJsonAsync($"{Client.BaseAddress}api/referrals", referralDto, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new ReferralClientServiceException(response, await response.Content.ReadAsStringAsync(cancellationToken));
        }

        ReferralResponse? referralResponse = await response.Content.ReadFromJsonAsync<ReferralResponse>(cancellationToken: cancellationToken);

        if (referralResponse is null)
        {
            // the only time it'll be null, is if the API returns "null"
            // (see https://stackoverflow.com/questions/71162382/why-are-the-return-types-of-nets-system-text-json-jsonserializer-deserialize-m)
            // unlikely, but possibly (pass new MemoryStream(Encoding.UTF8.GetBytes("null")) to see it actually return null)
            // note we hard-code passing "null", rather than messing about trying to rewind the stream, as this is such a corner case and we want to let the deserializer take advantage of the async stream (in the happy case)
            throw new ReferralClientServiceException(response, "null");
        }

        return referralResponse;
    }
}
