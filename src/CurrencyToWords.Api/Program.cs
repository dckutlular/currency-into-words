using CurrencyToWords.Contracts;
using CurrencyToWords.Core;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();

// Converters are stateless, so one instance each is enough. Add a language by registering another one.
builder.Services.AddSingleton<ICurrencyToWordsConverter, EnglishCurrencyToWordsConverter>();
builder.Services.AddSingleton<ICurrencyToWordsConverter, GermanCurrencyToWordsConverter>();

// The Blazor client runs on a different origin, so the browser needs CORS permission to call the API.
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy
    .WithOrigins(allowedOrigins)
    .WithMethods("POST")
    .WithHeaders("Content-Type")));

var app = builder.Build();

// Unhandled exceptions become a generic 500 ProblemDetails; details are only logged.
app.UseExceptionHandler();
app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// No UseHttpsRedirection: browsers don't follow redirects on CORS preflight requests,
// so a redirect would block the Blazor client. HTTPS is the hosting environment's job.

// The API also serves the Blazor client, so a deployment is one service with one address.
// The client still runs in the browser and calls /api/convert over HTTP.
app.MapStaticAssets();

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
.WithName("Convert")
// A valid request is a few dozen bytes. Without a limit, Kestrel accepts 30 MB per request,
// and a few parallel large requests can use up the memory of a small hosting instance.
.WithMetadata(new RequestSizeLimitAttribute(4 * 1024));

// Any other page URL is a client-side route, so it gets the Blazor app.
// Unknown /api addresses get 404 instead, so API clients never receive the HTML page.
app.MapFallback("/api/{**path}", () => Results.NotFound());
app.MapFallbackToFile("index.html");

app.Run();
