namespace FamilyHubs.Referral.Web.Extensions;

public static class StringExtensions
{
    //todo: move to file in core?
    public static string ToSentenceCase(this string input)
    {
        if (input.Length < 1)
            return input;

        var sentence = input.ToLower();
        return $"{sentence[0].ToString().ToUpper()}{sentence.AsSpan(1)}";
    }
}
