# Builds the API, which also serves the Blazor client, into one container image.

# --- Build stage: the SDK image compiles and publishes the app ---
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore first, from the project files only, so this layer is reused
# as long as no package or project reference changes.
COPY src/CurrencyToWords.Api/CurrencyToWords.Api.csproj src/CurrencyToWords.Api/
COPY src/CurrencyToWords.Core/CurrencyToWords.Core.csproj src/CurrencyToWords.Core/
COPY src/CurrencyToWords.Contracts/CurrencyToWords.Contracts.csproj src/CurrencyToWords.Contracts/
COPY src/CurrencyToWords.Web/CurrencyToWords.Web.csproj src/CurrencyToWords.Web/
RUN dotnet restore src/CurrencyToWords.Api/CurrencyToWords.Api.csproj

COPY src/ src/
RUN dotnet publish src/CurrencyToWords.Api/CurrencyToWords.Api.csproj -c Release -o /app --no-restore

# --- Runtime stage: only the ASP.NET runtime and the published app ---
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .

# Run as the image's built-in non-root user.
USER $APP_UID

# The .NET images listen on port 8080 by default.
EXPOSE 8080
ENTRYPOINT ["dotnet", "CurrencyToWords.Api.dll"]
