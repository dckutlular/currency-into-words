namespace CurrencyToWords.Core;

public sealed class GermanCurrencyToWordsConverter : ICurrencyToWordsConverter
{
    private static readonly string[] UpToNineteen =
    [
        "null", "eins", "zwei", "drei", "vier", "fünf", "sechs", "sieben", "acht", "neun",
        "zehn", "elf", "zwölf", "dreizehn", "vierzehn", "fünfzehn", "sechzehn", "siebzehn", "achtzehn", "neunzehn",
    ];

    private static readonly string[] Tens =
        ["", "", "zwanzig", "dreißig", "vierzig", "fünfzig", "sechzig", "siebzig", "achtzig", "neunzig"];

    public string Language => "de";

    public string Convert(CurrencyAmount amount)
    {
        var dollars = $"{CountWord(amount.Dollars)} Dollar";

        if (amount.Cents == 0)
            return dollars;

        return $"{dollars} und {CountWord(amount.Cents)} Cent";
    }

    // "Dollar" and "Cent" have no plural form, but exactly one is "ein Dollar", not "eins Dollar".
    private static string CountWord(int number) => number == 1 ? "ein" : NumberToWords(number);

    private static string NumberToWords(int number)
    {
        if (number == 0)
            return "null";

        var millions = number / 1_000_000;
        var thousands = number / 1_000 % 1_000;
        var rest = number % 1_000;

        var parts = new List<string>();
        if (millions == 1)
            parts.Add("eine Million");
        else if (millions > 1)
            parts.Add($"{BelowThousand(millions, trailingOne: "eins")} Millionen");

        // Everything below a million is written as one word.
        var belowMillion = (thousands > 0 ? $"{BelowThousand(thousands, trailingOne: "ein")}tausend" : "")
            + BelowThousand(rest, trailingOne: "eins");
        if (belowMillion.Length > 0)
            parts.Add(belowMillion);

        return string.Join(" ", parts);
    }

    /// <param name="number">0 to 999; 0 gives an empty string.</param>
    /// <param name="trailingOne">Word for a 1 at the end: "eins" (einhunderteins) or "ein" before "tausend".</param>
    private static string BelowThousand(int number, string trailingOne)
    {
        var hundreds = number / 100;
        var hundredsWord = hundreds > 0 ? $"{UnitAsPrefix(hundreds)}hundert" : "";
        return hundredsWord + BelowHundred(number % 100, trailingOne);
    }

    // The unit comes first: 25 -> "fünfundzwanzig", 21 -> "einundzwanzig".
    private static string BelowHundred(int number, string trailingOne)
    {
        if (number == 0)
            return "";

        if (number == 1)
            return trailingOne;

        if (number < 20)
            return UpToNineteen[number];

        var unit = number % 10;
        return unit == 0 ? Tens[number / 10] : $"{UnitAsPrefix(unit)}und{Tens[number / 10]}";
    }

    // Inside a compound word, 1 is "ein": "einhundert", "einundzwanzig".
    private static string UnitAsPrefix(int unit) => unit == 1 ? "ein" : UpToNineteen[unit];
}
