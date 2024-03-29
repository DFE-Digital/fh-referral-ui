﻿using FamilyHubs.SharedKernel.Razor.FamilyHubsUi.Options;
using Microsoft.Extensions.Options;

namespace FamilyHubs.Referral.Web.Pages.Shared;

public class FamilyHubsUiPageModel : HeaderPageModel
{
    public IFamilyHubsUiOptions FamilyHubsUiOptions { get; }

    public FamilyHubsUiPageModel(IOptions<FamilyHubsUiOptions> familyHubsUiOptions)
    {
        FamilyHubsUiOptions = familyHubsUiOptions.Value;
    }
}