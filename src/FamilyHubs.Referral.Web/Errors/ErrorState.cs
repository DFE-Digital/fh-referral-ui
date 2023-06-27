using FamilyHubs.Referral.Core.Models;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace FamilyHubs.Referral.Web.Errors;

//todo: combine error handling from base class into this too?
public class ErrorState : IErrorSummary
{
    private readonly ImmutableDictionary<int, Error> _possibleErrors;

    public ErrorState(ImmutableDictionary<int, Error> possibleErrors, IEnumerable<int> triggeredErrors)
    {
        _possibleErrors = possibleErrors;
        ErrorIds = triggeredErrors;
    }

    //todo: generic concrete, but non-generic interface?
    public static ErrorState Create<T>(ImmutableDictionary<int, Error> possibleErrors, IEnumerable<T>? triggeredErrors)
        where T : struct, Enum, IConvertible
    {
        return new ErrorState(possibleErrors, triggeredErrors?.Select(e => (int)(IConvertible)e) ?? Enumerable.Empty<int>());
    }

    public bool HasErrors => ErrorIds.Any();

    public IEnumerable<int> ErrorIds { get; }

    public Error GetError(int errorId)
    {
        return _possibleErrors[errorId];
    }

    public bool HasError(params int[] errorIds)
    {
        return GetErrorIdIfTriggered(errorIds) != null;
    }

    [SuppressMessage("Minor Code Smell", "S3267:Loops should be simplified with \"LINQ\" expressions", Justification = "LINQ expression version is less simple")]
    public int? GetErrorIdIfTriggered(params int[] mutuallyExclusiveErrorIds)
    {
        if (!mutuallyExclusiveErrorIds.Any())
        {
            return ErrorIds.Any() ? ErrorIds.First() : null;
        }

        foreach (int errorId in mutuallyExclusiveErrorIds)
        {
            if (ErrorIds.Contains(errorId))
            {
                return errorId;
            }
        }

        return null;
    }

    public Error? GetErrorIfTriggered(params int[] mutuallyExclusiveErrorIds)
    {
        int? currentErrorId = GetErrorIdIfTriggered(mutuallyExclusiveErrorIds);
        return currentErrorId != null ? GetError(currentErrorId.Value) : null;
    }
}