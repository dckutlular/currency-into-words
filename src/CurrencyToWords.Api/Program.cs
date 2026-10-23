using CurrencyToWords.Api;
using CurrencyToWords.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

// Converters are stateless, so one instance each is enough. Add a language by registering another one.
builder.Services.AddSingleton<ICurrencyToWordsConverter, EnglishCurrencyToWordsConverter>();
builder.Services.AddSingleton<ICurrencyToWordsConverter, GermanCurrencyToWordsConverter>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/api/convert", (ConvertRequest request, IEnumerable<ICurrencyToWordsConverter> converters) =>
{
    var converter = converters.Single(c => c.Language == request.Language);
    var amount = AmountParser.Parse(request.Amount, request.Language);

    return new ConvertResponse(converter.Convert(amount));
})
.WithName("Convert");

app.Run();
