using System.Net;

namespace FamilyHubs.Referral.Infrastructure.Notifications;
// at some point we might want a more generic HttpClientException / RestHttpClientException
// or at least a common base class with ServiceDirectoryClientException

// ignore Sonar's "Update this implementation of 'ISerializable' to confirm to the recommended serialization pattern" (https://rules.sonarsource.com/csharp/RSPEC-3925)
// .Net Core itself doesn't implement serialization on most exceptions, see https://github.com/dotnet/runtime/issues/21433#issue-225189643
#pragma warning disable S3925
public class NotificationsClientException : Exception
{
    public HttpStatusCode? StatusCode { get; }
    public string? ReasonPhrase { get; }
    public Uri? RequestUri { get; }
    public string? ErrorResponse { get; }

    public NotificationsClientException(HttpResponseMessage httpResponseMessage, string errorResponse)
        : base(GenerateMessage(httpResponseMessage, errorResponse))
    {
        StatusCode = httpResponseMessage.StatusCode;
        ReasonPhrase = httpResponseMessage.ReasonPhrase;
        RequestUri = httpResponseMessage.RequestMessage!.RequestUri;
        ErrorResponse = errorResponse;
    }

    private static string GenerateMessage(HttpResponseMessage httpResponseMessage, string errorResponse)
    {
        return $@"Request '{httpResponseMessage.RequestMessage?.RequestUri}'
                    returned {(int)httpResponseMessage.StatusCode} {httpResponseMessage.ReasonPhrase}
                    Response: {errorResponse}";
    }
}