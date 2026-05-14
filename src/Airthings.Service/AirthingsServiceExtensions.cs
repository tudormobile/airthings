using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Tudormobile.Airthings.Service;

/// <summary>
/// Extension methods for configuring Airthings Service endpoints.
/// </summary>
public static class AirthingsServiceExtensions
{
    /// <summary>
    /// Registers services required by the Airthings Service, including the typed HTTP client and options.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration used to bind <see cref="AirthingsOptions"/>.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddAirthingsService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<AirthingsOptions>()
            .Configure(opts =>
            {
                var section = configuration.GetSection("Airthings");
                opts.ApiKey       = section["ApiKey"]       ?? Environment.GetEnvironmentVariable("AIRTHINGS_API_KEY")       ?? string.Empty;
                opts.ClientId     = section["ClientId"]     ?? Environment.GetEnvironmentVariable("AIRTHINGS_CLIENT_ID")     ?? string.Empty;
                opts.ClientSecret = section["ClientSecret"] ?? Environment.GetEnvironmentVariable("AIRTHINGS_CLIENT_SECRET") ?? string.Empty;
            });

        services.AddHttpClient<AirthingsClient>((sp, client) =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);
        })
        .AddTypedClient((client, sp) =>
        {
            var opts = sp.GetRequiredService<IOptions<AirthingsOptions>>().Value;
            return new AirthingsClient(client, opts.ClientId, opts.ClientSecret);
        });

        services.AddOutputCache();
        services.AddAuthorization();
        return services;
    }

    public static WebApplication UseAirthingsService(this WebApplication app)
    {
        var prefix = "/home/airthings/v1";

        var airthingsClient = app.Services.GetRequiredService<AirthingsClient>();
        var opts = app.Services.GetRequiredService<IOptions<AirthingsOptions>>().Value;
        var api = new AirthingsApi(
            opts.ApiKey,
            airthingsClient,
            app.Logger,
            app.Environment);

        // Map Airthings endpoints
        app.MapGet($"{prefix}/status", (HttpContext context, [FromHeader(Name = "ApiKey")] string? apiKey)
            => api.GetVersionAsync(context, apiKey ?? string.Empty)).CacheOutput(p => p.Expire(TimeSpan.FromHours(1)));

        app.MapGet($"{prefix}/devices", (HttpContext context, [FromHeader(Name = "ApiKey")] string? apiKey)
            => api.GetDevicesAsync(context, apiKey ?? string.Empty)).CacheOutput(p => p.Expire(TimeSpan.FromHours(1)));

        app.MapGet($"{prefix}/samples", (HttpContext context, [FromHeader(Name = "ApiKey")] string? apiKey)
            => api.GetSamplesAsync(context, apiKey ?? string.Empty)).CacheOutput(p => p.Expire(TimeSpan.FromMinutes(30)));

        app.Logger.LogInformation("AirthingsService, Running, {Prefix}", prefix);
        return app;
    }
}
