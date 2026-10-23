using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using CurrencyToWords.Web;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Deployed, the API serves this app, so it calls its own address (empty ApiBaseUrl).
// In local development the API runs separately; appsettings.Development.json points to it.
var apiBaseUrl = builder.Configuration["ApiBaseUrl"];
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(string.IsNullOrEmpty(apiBaseUrl) ? builder.HostEnvironment.BaseAddress : apiBaseUrl),
});

await builder.Build().RunAsync();
