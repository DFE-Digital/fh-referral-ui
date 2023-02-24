namespace FamilyHubs.ReferralUi.Ui.Extensions;

public static class StringExtensions
{
    public static string ToSentenceCase(this string input)
    {
        if (input.Length < 1)
            return input;

        string sentence = input.ToLower();
        return $"{sentence[0].ToString().ToUpper()}{sentence.AsSpan(1)}";
    }
}
