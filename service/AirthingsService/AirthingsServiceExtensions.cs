using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Tudormobile.AirthingsService;

/// <summary>
/// Extension methods for configuring Airthings Service endpoints.
/// </summary>
public static class AirthingsServiceExtensions
{
    /// <summary>
    /// Configures and maps Airthings Service API endpoints to the application.
    /// </summary>
    /// <param name="app">The web application to configure.</param>
    /// <returns>The configured web application for method chaining.</returns>
    public static WebApplication UseAirthingsService(this WebApplication app)
    {
        var prefix = "/airthings/v1";

        // Get the JsonOptions from DI
        var jsonOptions = app.Services.GetRequiredService<IOptions<JsonOptions>>();

        // Airthings AUTH
        var clientId = app.Configuration.GetSection("Airthings")["ClientId"] ?? Environment.GetEnvironmentVariable("AIRTHINGS_CLIENT_ID") ?? string.Empty;
        var clientSecret = app.Configuration.GetSection("Airthings")["ClientSecret"] ?? Environment.GetEnvironmentVariable("AIRTHINGS_CLIENT_SECRET") ?? string.Empty;

        var api = new AirthingsApi(
            app.Configuration.GetSection("Airthings")["ApiKey"] ?? string.Empty,
            clientId, clientSecret,
            app.Logger,
            app.Environment, jsonOptions?.Value.JsonSerializerOptions ?? new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        // Map Airthings endpoints
        app.MapGet($"{prefix}/status", (HttpContext context, [FromHeader(Name = "ApiKey")] string? apiKey)
            => api.GetVersionAsync(context, apiKey ?? string.Empty)).CacheOutput(p => p.Expire(TimeSpan.FromHours(1)));

        app.Logger.LogInformation("AirthingsService, Running, {0}", prefix);
        return app;
    }
}
