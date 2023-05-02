
namespace FamilyHubs.Referral.Web.Models;

public interface ISingleTextboxPageModel
{
    string HeadingText { get; set; }
    string? HintText { get; set; }
    string TextBoxLabel { get; set; }
    string ErrorText { get; set; }
    string? TextBoxValue { get; set; }
    bool ValidationValid { get; set; }
}

