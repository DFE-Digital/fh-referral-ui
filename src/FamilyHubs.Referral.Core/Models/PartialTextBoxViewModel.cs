using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.Referral.Core.Models;

public class PartialTextBoxViewModel
{
    public string HeadingText { get; set; } = default!;
    public string HintText { get; set; } = default!;
    public string TextBoxLabel { get; set; } = default!;
    public string MainErrorText { get; set; } = default!;
    public string TextBoxErrorText { get; set; } = default!;
    public string TextBoxValue { get; set; } = string.Empty;
    public bool ValidationValid { get; set; } = true;
}
