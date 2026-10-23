using System.Net;
using System.Net.Http.Json;
using CurrencyToWords.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CurrencyToWords.Api.Tests;

/// <summary>
/// Calls POST /api/convert over HTTP against the API running in memory, like the Blazor client does.
/// </summary>
public class ConvertEndpointTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    [Theory]
    [InlineData("en", "twenty-five dollars and ten cents")]
    [InlineData("de", "fünfundzwanzig Dollar und zehn Cent")]
    public async Task Converts_amount_in_requested_language(string language, string expected)
    {
        var response = await factory.CreateClient().PostAsJsonAsync("/api/convert", new ConvertRequest("25,1", language));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ConvertResponse>();
        Assert.Equal(expected, body?.Words);
    }

    [Fact]
    public async Task Rejects_invalid_amount_with_message_in_requested_language()
    {
        var response = await factory.CreateClient().PostAsJsonAsync("/api/convert", new ConvertRequest("25,999", "en"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.Equal("Invalid amount", problem?.Title);
        Assert.Equal("Cents can have at most two digits.", problem?.Detail);
    }

    [Fact]
    public async Task Rejects_unsupported_language()
    {
        var response = await factory.CreateClient().PostAsJsonAsync("/api/convert", new ConvertRequest("1", "fr"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.Equal("Unsupported language", problem?.Title);
        Assert.Equal("Language 'fr' is not supported. Use one of: en, de.", problem?.Detail);
    }

    [Fact]
    public async Task Rejects_oversized_request_before_reading_it()
    {
        // The in-memory test server ignores body size limits, so this test runs the API on real Kestrel.
        using var kestrel = new WebApplicationFactory<Program>();
        kestrel.UseKestrel(0);
        kestrel.StartServer();

        // A real amount is a few dozen bytes; big bodies would only cost server memory.
        var oversized = new ConvertRequest(new string(' ', 5_000), "en");

        var response = await kestrel.CreateClient().PostAsJsonAsync("/api/convert", oversized);

        Assert.Equal(HttpStatusCode.RequestEntityTooLarge, response.StatusCode);
    }
}
