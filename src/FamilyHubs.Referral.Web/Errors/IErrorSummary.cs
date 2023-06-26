namespace FamilyHubs.Referral.Web.Errors;

public record Error(int Id, string HtmlElementId, string ErrorMessage);

//todo: need a composable helper class to implement this interface
public interface IErrorSummary
{
    bool HasErrors { get; }
    IEnumerable<int> ErrorIds { get; }
    Error GetError(int errorId);

    //bool HasTriggeredError(int errorId);
    bool HasTriggeredError(params int[] errorIds);
    int? GetErrorIdIfTriggered(params int[] mutuallyExclusiveErrorIds);

    Error? GetErrorIfTriggered(params int[] mutuallyExclusiveErrorIds);
    //int? GetCurrentErrorId(params int[] mutuallyExclusiveErrorIds);
    //Error? GetCurrentError(params int[] mutuallyExclusiveErrorIds);
}