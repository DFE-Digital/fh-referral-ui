﻿using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.Referral.Web.Models;

public interface ITellTheServicePageModel
{
    string DescriptionPartial { get; }

    [BindProperty]
    string? TextAreaValue { get; set; }

    string? TextAreaValidationErrorMessage { get; set; }
}