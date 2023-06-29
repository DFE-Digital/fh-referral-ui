using FamilyHubs.Referral.Web.Errors;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.Referral.Web.Models;

public interface ITellTheServicePageModel
{
    string DescriptionPartial { get; }

    [BindProperty]
    string? TextAreaValue { get; set; }

    // might be cleaner to put back a TellTheServiceValidationStatus enum (Valid|Empty|TooLong)
    // and add the error messages to the interface (especially if we have consistent error messages constructed from the h1/title)
    // also if we add a new base class for the TellTheService pages
    //string? TextAreaValidationErrorMessage { get; set; }

    //todo: non-nullable (would prob need a static blank ErrorState set in the ctor)
    public ErrorState? ErrorState { get; }
}