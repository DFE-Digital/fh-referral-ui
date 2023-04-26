using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.Referral.Core.Models;

public class PartialTextBoxViewModel
{
    public required string ErrorId { get; init; }
    public required string HeadingText { get; set; }
    public required string HintText { get; set; }
    public required string TextBoxLabel { get; set; }
    public required string MainErrorText { get; set; }
    public required string TextBoxErrorText { get; set; }
    public string? TextBoxValue { get; set; }
    public bool ValidationValid { get; set; } = true;
}
