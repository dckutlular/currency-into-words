namespace CurrencyToWords.Core.Tests;

public class AmountParserTests
{
    [Theory]
    [InlineData("0", 0, 0)]
    [InlineData("45 100", 45_100, 0)]
    [InlineData("25,1", 25, 10)]
    [InlineData("0,01", 0, 1)]
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
    [InlineData("en", "Cents must be one or two digits after the comma, for example 25,1 or 25,10.")]
    [InlineData("de", "Cent müssen ein oder zwei Ziffern nach dem Komma sein, zum Beispiel 25,1 oder 25,10.")]
    [InlineData("fr", "Cents must be one or two digits after the comma, for example 25,1 or 25,10.")]
    public void Error_message_uses_selected_language(string language, string expected)
    {
        var exception = Assert.Throws<FormatException>(() => AmountParser.Parse("25,999", language));

        Assert.Equal(expected, exception.Message);
    }
}
