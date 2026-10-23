namespace CurrencyToWords.Core;

public interface ICurrencyToWordsConverter
{
    /// <summary>Language code this converter writes, e.g. "en" or "de".</summary>
    string Language { get; }

    string Convert(CurrencyAmount amount);
}
