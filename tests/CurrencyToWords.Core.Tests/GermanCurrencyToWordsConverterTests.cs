namespace CurrencyToWords.Core.Tests;

public class GermanCurrencyToWordsConverterTests
{
    private readonly GermanCurrencyToWordsConverter _converter = new();

    [Theory]
    [InlineData(0, 0, "null Dollar")]
    [InlineData(1, 0, "ein Dollar")]
    [InlineData(7, 0, "sieben Dollar")]
    [InlineData(16, 0, "sechzehn Dollar")]
    [InlineData(17, 0, "siebzehn Dollar")]
    [InlineData(21, 0, "einundzwanzig Dollar")]
    [InlineData(25, 0, "fünfundzwanzig Dollar")]
    [InlineData(30, 0, "dreißig Dollar")]
    [InlineData(99, 0, "neunundneunzig Dollar")]
    [InlineData(100, 0, "einhundert Dollar")]
    [InlineData(101, 0, "einhunderteins Dollar")]
    [InlineData(45_100, 0, "fünfundvierzigtausendeinhundert Dollar")]
    [InlineData(1_000_000, 0, "eine Million Dollar")]
    [InlineData(101_000_000, 0, "einhunderteins Millionen Dollar")]
    [InlineData(2_000_000, 0, "zwei Millionen Dollar")]
    [InlineData(1_000_001, 0, "eine Million eins Dollar")]
    [InlineData(0, 1, "null Dollar und ein Cent")]
    [InlineData(25, 10, "fünfundzwanzig Dollar und zehn Cent")]
    [InlineData(1, 1, "ein Dollar und ein Cent")]
    [InlineData(999_999_999, 99,
        "neunhundertneunundneunzig Millionen neunhundertneunundneunzigtausendneunhundertneunundneunzig Dollar und neunundneunzig Cent")]
    public void Converts_amount_to_words(int dollars, int cents, string expected)
    {
        Assert.Equal(expected, _converter.Convert(new CurrencyAmount(dollars, cents)));
    }
}
