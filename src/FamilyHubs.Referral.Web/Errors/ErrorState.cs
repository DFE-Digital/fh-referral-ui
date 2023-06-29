using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace FamilyHubs.Referral.Web.Errors;

public class ErrorState<T> : ErrorState, IErrorState<T>
    where T : struct, Enum, IConvertible
{
    public ErrorState(ImmutableDictionary<int, Error> possibleErrors, IEnumerable<int> triggeredErrors)
        : base(possibleErrors, triggeredErrors)
    {
    }

    public new IEnumerable<T> ErrorIds => base.ErrorIds.Select(e => (T) (IConvertible) e);

    public Error GetError(T errorId)
    {
        return GetError((int) (IConvertible) errorId);
    }

    public bool HasError(params T[] errorIds)
    {
        return HasError(errorIds.Select(e => (int) (IConvertible) e).ToArray());
    }

    public T? GetErrorIdIfTriggered(params T[] mutuallyExclusiveErrorIds)
    {
        return (T?) (IConvertible?) GetErrorIdIfTriggered(mutuallyExclusiveErrorIds.Select(e => (int) (IConvertible) e).ToArray());
    }

    public Error? GetErrorIfTriggered(params T[] mutuallyExclusiveErrorIds)
    {
        return GetErrorIfTriggered(mutuallyExclusiveErrorIds.Select(e => (int) (IConvertible) e).ToArray());
    }
}

//todo: combine error handling from base class into this too?
public class ErrorState : IErrorState
{
    private readonly ImmutableDictionary<int, Error> _possibleErrors;

    public ErrorState(ImmutableDictionary<int, Error> possibleErrors, IEnumerable<int> triggeredErrors)
    {
        _possibleErrors = possibleErrors;
        ErrorIds = triggeredErrors;
    }

    public static IErrorState Empty { get; }
        = new ErrorState(ImmutableDictionary<int, Error>.Empty, Enumerable.Empty<int>());

    public static IErrorState Create<T>(ImmutableDictionary<int, Error> possibleErrors, IEnumerable<T>? triggeredErrors)
        where T : struct, Enum, IConvertible
    {
        if (triggeredErrors?.Any() == true)
        {
            return new ErrorState(possibleErrors,
                triggeredErrors.Select(e => (int) (IConvertible) e));
        }

        return Empty;
    }

    public bool HasErrors => ErrorIds.Any();

    //todo: either/and IEnumerable<Error>?
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