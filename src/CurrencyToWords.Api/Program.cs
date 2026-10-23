using CurrencyToWords.Api;
using CurrencyToWords.Core;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();

// Converters are stateless, so one instance each is enough. Add a language by registering another one.
builder.Services.AddSingleton<ICurrencyToWordsConverter, EnglishCurrencyToWordsConverter>();
builder.Services.AddSingleton<ICurrencyToWordsConverter, GermanCurrencyToWordsConverter>();

var app = builder.Build();

// Unhandled exceptions become a generic 500 ProblemDetails; details are only logged.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/api/convert", Results<Ok<ConvertResponse>, ProblemHttpResult> (
    ConvertRequest request, IEnumerable<ICurrencyToWordsConverter> converters) =>
{
    // Checked before parsing, so the parser only ever sees a supported language.
    var converter = converters.FirstOrDefault(c => c.Language == request.Language);
    if (converter is null)
    {
        var supported = string.Join(", ", converters.Select(c => c.Language));
        return TypedResults.Problem(
            statusCode: StatusCodes.Status400BadRequest,
            title: "Unsupported language",
            detail: $"Language '{request.Language}' is not supported. Use one of: {supported}.");
    }

    CurrencyAmount amount;
    try
    {
        amount = AmountParser.Parse(request.Amount, request.Language);
    }
    catch (FormatException ex)
    {
        // The parser's message is written for the user, in the requested language.
        return TypedResults.Problem(
            statusCode: StatusCodes.Status400BadRequest,
            title: "Invalid amount",
            detail: ex.Message);
    }

    return TypedResults.Ok(new ConvertResponse(converter.Convert(amount)));
})
.WithName("Convert");

app.Run();
