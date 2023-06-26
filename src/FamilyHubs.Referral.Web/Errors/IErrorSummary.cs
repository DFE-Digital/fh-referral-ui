namespace FamilyHubs.Referral.Web.Errors;

public record Error(int Id, string HtmlElementId, string ErrorMessage);

//todo: need a composable helper class to implement this interface
public interface IErrorSummary
{
    //todo: better naming of all these. GetError is the odd one out as it's not referring to the current state
    bool HasErrors { get; }
    bool HasError(params int[] errorIds);

    IEnumerable<int> ErrorIds { get; }

    int? GetErrorIdIfTriggered(params int[] mutuallyExclusiveErrorIds);
    Error? GetErrorIfTriggered(params int[] mutuallyExclusiveErrorIds);

    //todo: leave this out of interface? (GetErrorIfTriggered could be used instead (renamed), but consumers would have to deal with null) or just rename?
    Error GetError(int errorId);
}