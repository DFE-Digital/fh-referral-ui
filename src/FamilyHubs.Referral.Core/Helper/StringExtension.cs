namespace FamilyHubs.Referral.Core.Helper;

public static class StringExtension
{
    public static string Truncate(this string value, int maxLength, string truncationSuffix = "…")
    {
        return value.Length > maxLength
            ? value.Substring(0, maxLength) + truncationSuffix
            : value;
    }

    public static string ToSentenceCase(this string input)
    {
        if (input.Length < 1)
            return input;

        var sentence = input.ToLower();
        return $"{sentence[0].ToString().ToUpper()}{sentence.AsSpan(1)}";
    }
}