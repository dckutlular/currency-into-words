namespace CurrencyToWords.Core.Tests;

public class AmountParserTests
{
    [Theory]
    [InlineData("0", 0, 0)]
    [InlineData("45 100", 45_100, 0)]
    [InlineData("25,1", 25, 10)]
    [InlineData("0,01", 0, 1)]
    [InlineData("0,33", 0, 33)]
    [InlineData("999 999 999,99", 999_999_999, 99)]
    public void Parses_valid_input(string input, int dollars, int cents)
    {
        Assert.Equal(new CurrencyAmount(dollars, cents), AmountParser.Parse(input, "en"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("abc")]
    [InlineData("-5")]
    [InlineData("25.1")]
    [InlineData(",5")]
    [InlineData("25,")]
    [InlineData("25,999")]
    [InlineData("1,2,3")]
    [InlineData("1 000 000 000")]
    [InlineData("99999999999999999999")]
    public void Rejects_invalid_input(string input)
    {
        Assert.Throws<FormatException>(() => AmountParser.Parse(input, "en"));
    }

    [Theory]
    [InlineData("25,", "en", "The cent part after ',' is missing.")]
    [InlineData("25,", "de", "Der Cent-Teil nach dem ',' fehlt.")]
    [InlineData("25,999", "en", "Cents can have at most two digits.")]
    [InlineData("25,999", "de", "Cent dürfen höchstens zwei Ziffern haben.")]
    [InlineData(",33", "en", "The dollar part before ',' is missing.")]
    [InlineData(",33", "de", "Der Dollar-Teil vor dem ',' fehlt.")]
    [InlineData("25,a", "en", "Only digits, spaces and a single ',' are allowed (e.g. 45 100,25).")]
    [InlineData("25,a", "de", "Nur Ziffern, Leerzeichen und ein einzelnes ',' sind erlaubt (z. B. 45 100,25).")]
    [InlineData("abc", "en", "Only digits, spaces and a single ',' are allowed (e.g. 45 100,25).")]
    [InlineData("25.1", "de", "Nur Ziffern, Leerzeichen und ein einzelnes ',' sind erlaubt (z. B. 45 100,25).")]
    [InlineData("25,999", "fr", "Cents can have at most two digits.")]
    public void Error_message_explains_the_problem_in_selected_language(string input, string language, string expected)
    {
        var exception = Assert.Throws<FormatException>(() => AmountParser.Parse(input, language));

        Assert.Equal(expected, exception.Message);
    }
}
