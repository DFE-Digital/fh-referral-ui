namespace FamilyHubs.Referral.Web.Errors;

public record Error(int Id, string HtmlElementId, string ErrorMessage);

//todo: better naming of all these. GetError is the odd one out as it's not referring to the current state
//todo: single interface? copy methods? better naming?

public interface IErrorSummary
{
    bool HasErrors { get; }
    IEnumerable<int> ErrorIds { get; }

    //todo: leave this out of interface? (GetErrorIfTriggered could be used instead (renamed), but consumers would have to deal with null) or just rename?
    Error GetError(int errorId);
}

public interface IErrorState : IErrorSummary
{
    bool HasError(params int[] errorIds);

    int? GetErrorIdIfTriggered(params int[] mutuallyExclusiveErrorIds);
    Error? GetErrorIfTriggered(params int[] mutuallyExclusiveErrorIds);
}