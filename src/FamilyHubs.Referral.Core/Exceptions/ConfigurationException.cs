namespace FamilyHubs.Referral.Core.Exceptions;

#pragma warning disable S3925
public class ConfigurationException : Exception
{
    public ConfigurationException(string key, string? value, string? expected = null, string? example = null)
        : base(CreateMessage(key, value, expected, example))
    {
    }

    public ConfigurationException(string key, string? value, Exception? innerException, string? expected = null, string? example = null)
        : base(CreateMessage(key, value, expected, example), innerException)
    {
    }

    private static string CreateMessage(string key, string? value, string? expected, string? example)
    {
        return $"""
Configuration issue
Key      : "{key}"
Found    : "{value}"
Expected : {expected}
Example  : "{example}"
""";
    }

    //todo: configuration helper somewhere that gets from config and throws if issue
    /// <returns>null safe url</returns>
    /// <exception cref="ConfigurationException"></exception>
    public static string ThrowIfNotUrl(
        string key,
        string? url,
        string? expected = null,
        string? example = "http://example.com",
        UriKind uriKind = UriKind.RelativeOrAbsolute)
    {
        if (string.IsNullOrWhiteSpace(url) || !Uri.IsWellFormedUriString(url, uriKind))
        {
            throw new ConfigurationException(key, url, expected, example);
        }

        return url;
    }
}