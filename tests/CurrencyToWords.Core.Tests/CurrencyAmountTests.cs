namespace CurrencyToWords.Core.Tests;

public class CurrencyAmountTests
{
    [Theory]
    [InlineData(0, 0)]
    [InlineData(25, 10)]
    [InlineData(999_999_999, 99)]
    public void Accepts_amounts_within_range(int dollars, int cents)
    {
        var amount = new CurrencyAmount(dollars, cents);

        Assert.Equal(dollars, amount.Dollars);
        Assert.Equal(cents, amount.Cents);
    }

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(1_000_000_000, 0)]
    [InlineData(0, -1)]
    [InlineData(0, 100)]
    public void Rejects_amounts_out_of_range(int dollars, int cents)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new CurrencyAmount(dollars, cents));
    }
}
