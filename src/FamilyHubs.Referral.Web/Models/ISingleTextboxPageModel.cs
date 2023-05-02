using System.ComponentModel.DataAnnotations;

namespace FamilyHubs.Referral.Web.Models;

//todo: best place for this to live?
public interface ISingleTextboxPageModel
{
    string HeadingText { get; set; }
    string? HintText { get; set; }
    string TextBoxLabel { get; set; }
    string MainErrorText { get; set; }
    //todo: default to mainerror
    string? TextBoxErrorText { get; set; }
    //two separate interfaces (with a common base) or override
    //but browser handles validation by the looks of it, so should we use email type anyway?
    //also, add autocomplete="email"
    //[EmailAddress]
    string? TextBoxValue { get; set; }
    bool ValidationValid { get; set; }
}

public interface ISingleEmailTextboxPageModel : ISingleTextboxPageModel
{
    [EmailAddress]
    new string? TextBoxValue { get; set; }
}

//public interface ISingleTextTextboxPageModel : ISingleTextboxPageModel
//{
//    public string? TextBoxValue { get; set; }
//}