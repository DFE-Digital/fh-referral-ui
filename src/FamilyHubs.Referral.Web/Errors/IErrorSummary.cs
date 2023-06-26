namespace FamilyHubs.Referral.Web.Errors;

public record Error(int Id, string HtmlElementId, string ErrorMessage);

//todo: need a composable helper class to implement this interface
public interface IErrorSummary
{
    //todo: better naming of all these. GetError is the odd one out as it's not referring to the current state
    bool HasErrors { get; }
    IEnumerable<int> ErrorIds { get; }
    Error GetError(int errorId);

    //bool HasTriggeredError(int errorId);
    bool HasTriggeredError(params int[] errorIds);
    int? GetErrorIdIfTriggered(params int[] mutuallyExclusiveErrorIds);

    Error? GetErrorIfTriggered(params int[] mutuallyExclusiveErrorIds);
}