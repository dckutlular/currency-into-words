namespace CurrencyToWords.Core.Tests;

public class EnglishCurrencyToWordsConverterTests
{
    private readonly EnglishCurrencyToWordsConverter _converter = new();

    [Theory]
    [InlineData(0, 0, "zero dollars")]
    [InlineData(1, 0, "one dollar")]
    [InlineData(7, 0, "seven dollars")]
    [InlineData(13, 0, "thirteen dollars")]
    [InlineData(20, 0, "twenty dollars")]
    [InlineData(25, 0, "twenty-five dollars")]
    [InlineData(99, 0, "ninety-nine dollars")]
    [InlineData(100, 0, "one hundred dollars")]
    [InlineData(101, 0, "one hundred one dollars")]
    [InlineData(45_100, 0, "forty-five thousand one hundred dollars")]
    [InlineData(1_000_000, 0, "one million dollars")]
    [InlineData(1_000_001, 0, "one million one dollars")]
    [InlineData(0, 1, "zero dollars and one cent")]
    [InlineData(25, 10, "twenty-five dollars and ten cents")]
    [InlineData(1, 1, "one dollar and one cent")]
    [InlineData(999_999_999, 99,
        "nine hundred ninety-nine million nine hundred ninety-nine thousand nine hundred ninety-nine dollars and ninety-nine cents")]
    public void Converts_amount_to_words(int dollars, int cents, string expected)
    {
        Assert.Equal(expected, _converter.Convert(new CurrencyAmount(dollars, cents)));
    }
}
