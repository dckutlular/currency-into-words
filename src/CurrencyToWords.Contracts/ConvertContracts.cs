namespace CurrencyToWords.Contracts;

/// <param name="Amount">User input as typed, e.g. "45 100" or "25,1". Parsing happens in Core.</param>
/// <param name="Language">"en" or "de".</param>
public sealed record ConvertRequest(string Amount, string Language);

public sealed record ConvertResponse(string Words);
