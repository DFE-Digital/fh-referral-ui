using FamilyHubs.SharedKernel.Razor.ErrorNext;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.Referral.Web.Models;

public interface ITellTheServicePageModel
{
    string DescriptionPartial { get; }

    [BindProperty]
    string? TextAreaValue { get; set; }

    public IErrorState Errors { get; }
}