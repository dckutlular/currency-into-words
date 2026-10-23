namespace CurrencyToWords.Core;

/// <summary>
/// A dollar amount that is always within the supported range.
/// </summary>
public sealed record CurrencyAmount
{
    public const int MaxDollars = 999_999_999;
    public const int MaxCents = 99;

    public int Dollars { get; }
    public int Cents { get; }

    public CurrencyAmount(int dollars, int cents)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(dollars);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(dollars, MaxDollars);
        ArgumentOutOfRangeException.ThrowIfNegative(cents);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(cents, MaxCents);

        Dollars = dollars;
        Cents = cents;
    }
}
