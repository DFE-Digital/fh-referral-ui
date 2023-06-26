namespace FamilyHubs.Referral.Web.Errors;

public record Error(string HtmlElementId, string ErrorMessage);

public interface IErrorSummary
{
    bool HasErrors { get; }
    IEnumerable<int> ErrorIds { get; }
    Error GetError(int errorId);
}