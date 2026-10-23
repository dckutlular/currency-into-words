namespace CurrencyToWords.Core;

public sealed class EnglishCurrencyToWordsConverter : ICurrencyToWordsConverter
{
    private static readonly string[] UpToNineteen =
    [
        "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine",
        "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen",
    ];

    private static readonly string[] Tens =
        ["", "", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety"];

    public string Language => "en";

    public string Convert(CurrencyAmount amount)
    {
        var dollars = $"{NumberToWords(amount.Dollars)} {(amount.Dollars == 1 ? "dollar" : "dollars")}";

        if (amount.Cents == 0)
            return dollars;

        return $"{dollars} and {UpToNinetyNine(amount.Cents)} {(amount.Cents == 1 ? "cent" : "cents")}";
    }

    private static string NumberToWords(int number)
    {
        if (number == 0)
            return "zero";

        var parts = new List<string>();
        AddGroup(parts, number / 1_000_000, "million");
        AddGroup(parts, number / 1_000 % 1_000, "thousand");
        AddGroup(parts, number % 1_000, scale: null);
        return string.Join(" ", parts);
    }

    // Adds a group of three digits, e.g. 45 -> "forty-five thousand". Empty groups add nothing.
    private static void AddGroup(List<string> parts, int group, string? scale)
    {
        if (group == 0)
            return;

        if (group >= 100)
            parts.Add($"{UpToNineteen[group / 100]} hundred");

        if (group % 100 > 0)
            parts.Add(UpToNinetyNine(group % 100));

        if (scale is not null)
            parts.Add(scale);
    }

    private static string UpToNinetyNine(int number)
    {
        if (number < 20)
            return UpToNineteen[number];

        var unit = number % 10;
        return unit == 0 ? Tens[number / 10] : $"{Tens[number / 10]}-{UpToNineteen[unit]}";
    }
}
