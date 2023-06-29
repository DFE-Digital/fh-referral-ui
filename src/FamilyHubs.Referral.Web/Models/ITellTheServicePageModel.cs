using FamilyHubs.Referral.Web.Errors;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.Referral.Web.Models;

public interface ITellTheServicePageModel
{
    string DescriptionPartial { get; }

    [BindProperty]
    string? TextAreaValue { get; set; }

    public IErrorState ErrorState { get; }
}