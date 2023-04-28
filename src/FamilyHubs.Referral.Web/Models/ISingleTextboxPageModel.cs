namespace FamilyHubs.Referral.Web.Models;

//todo: best place for this to live?
public interface ISingleTextboxPageModel
{
    //todo: get rid of text and label
    public string HeadingText { get; set; }
    public string? HintText { get; set; }
    public string TextBoxLabel { get; set; }
    public string MainErrorText { get; set; }
    //todo: default to mainerror
    public string? TextBoxErrorText { get; set; }
    public string? TextBoxValue { get; set; }
    public bool ValidationValid { get; set; }
}