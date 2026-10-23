using System.Globalization;
using CurrencyToWords.Core.Resources;

namespace CurrencyToWords.Core;

/// <summary>
/// Parses user input like "45 100" or "25,1" into a <see cref="CurrencyAmount"/>.
/// Spaces (any whitespace) are thousands separators and a comma separates dollars from cents.
/// </summary>
public static class AmountParser
{
    /// <param name="language">"de" for German messages; anything else gives English.</param>
    /// <exception cref="FormatException">The input is not a valid amount. The message can be shown to the user.</exception>
    public static CurrencyAmount Parse(string input, string language)
    {
        // Any whitespace counts as a thousands separator, including non-breaking spaces from copied text.
        var compact = string.Concat((input ?? "").Where(c => !char.IsWhiteSpace(c)));

        if (compact.Length == 0)
            throw Invalid(nameof(ValidationMessages.Empty), language);

        var parts = compact.Split(',');

        if (parts.Length > 2)
            throw Invalid(nameof(ValidationMessages.TooManyCommas), language);

        var dollars = ParseDollars(parts[0], language);
        var cents = parts.Length == 2 ? ParseCents(parts[1], language) : 0;

        return new CurrencyAmount(dollars, cents);
    }

    private static int ParseDollars(string text, string language)
    {
        // Only possible when the input starts with a comma, e.g. ",33". "0,33" is valid.
        if (text.Length == 0)
            throw Invalid(nameof(ValidationMessages.DollarsMissing), language);

        if (!IsDigitsOnly(text))
            throw Invalid(nameof(ValidationMessages.InvalidCharacters), language);

        // Digits only, so TryParse can only fail on overflow.
        if (!int.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out var dollars)
            || dollars > CurrencyAmount.MaxDollars)
            throw Invalid(nameof(ValidationMessages.TooLarge), language);

        return dollars;
    }

    private static int ParseCents(string text, string language)
    {
        if (text.Length == 0)
            throw Invalid(nameof(ValidationMessages.CentsMissing), language);

        if (!IsDigitsOnly(text))
            throw Invalid(nameof(ValidationMessages.InvalidCharacters), language);

        if (text.Length > 2)
            throw Invalid(nameof(ValidationMessages.TooManyCentDigits), language);

        // "25,1" means ten cents, like 25.1 written with a decimal point.
        var cents = int.Parse(text, CultureInfo.InvariantCulture);
        return text.Length == 1 ? cents * 10 : cents;
    }

    private static bool IsDigitsOnly(string text) => text.Length > 0 && text.All(char.IsAsciiDigit);

    // Unknown languages fall back to the English messages in ValidationMessages.resx.
    private static FormatException Invalid(string messageKey, string language) =>
        new(ValidationMessages.ResourceManager.GetString(messageKey, CultureInfo.GetCultureInfo(language)));
}
