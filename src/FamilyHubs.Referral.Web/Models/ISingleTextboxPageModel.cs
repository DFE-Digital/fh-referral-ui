using System.ComponentModel.DataAnnotations;

namespace FamilyHubs.Referral.Web.Models;

public interface ISingleTextboxPageModel
{
    string HeadingText { get; set; }
    string? HintText { get; set; }
    string TextBoxLabel { get; set; }
    string MainErrorText { get; set; }
    //todo: default to mainerror
    string? TextBoxErrorText { get; set; }
    string? TextBoxValue { get; set; }
    bool ValidationValid { get; set; }
}

