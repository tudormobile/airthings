using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Tudormobile.AirthingsService;

internal class AirthingsApi
{
    private readonly string _apiKey;
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly ILogger _logger;
    private readonly IWebHostEnvironment _env;
    private readonly JsonSerializerOptions _jsonOptions;

    public AirthingsApi(string apiKey, string clientId, string clientSecret, ILogger logger, IWebHostEnvironment env, JsonSerializerOptions jsonOptions)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiKey, nameof(apiKey));
        ArgumentException.ThrowIfNullOrWhiteSpace(clientId, nameof(clientId));
        ArgumentException.ThrowIfNullOrWhiteSpace(clientSecret, nameof(clientSecret));
        _apiKey = apiKey;
        _clientId = clientId;
        _clientSecret = clientSecret;
        _logger = logger;
        _env = env;
        _jsonOptions = jsonOptions;
    }

    internal Task<IResult> GetVersionAsync(HttpContext context, string apiKey)
        => HandleApiRequest(context, apiKey, nameof(GetVersionAsync), async () =>
        {
            using var client = new HttpClient();

            var tokenUrl = "https://accounts-api.airthings.com/v1/token";
            var tokenRequest = new HttpRequestMessage(HttpMethod.Post, tokenUrl);
            var content = JsonContent.Create(new
            {
                grant_type = "client_credentials",
                client_id = _clientId,
                client_secret = _clientSecret,
                scope = new string[] { "read:device:current_values" }
            });
            tokenRequest.Content = content;
            var tokenResponse = await client.SendAsync(tokenRequest);
            tokenResponse.EnsureSuccessStatusCode();
            var json = await tokenResponse.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var token = doc.RootElement.GetProperty("access_token").GetString() ?? "Unknown";


            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var result = await client.GetAsync("https://consumer-api.airthings.com/v1/accounts");
            var status = result.IsSuccessStatusCode ? "Healthy" : "Unhealthy";
            json = await result.Content.ReadAsStringAsync();
            using var doc2 = JsonDocument.Parse(json);

            var ids = doc2.RootElement
                        .GetProperty("accounts")
                        .EnumerateArray()
                        .Select(a => a.GetProperty("id").GetString() ?? string.Empty)
                        .ToList();

            foreach (var id in ids)
            {
                result = await client.GetAsync($"https://consumer-api.airthings.com/v1/accounts/{id}/devices");
                json = await result.Content.ReadAsStringAsync();
                using var doc3 = JsonDocument.Parse(json);

                var sns = doc3.RootElement
                            .GetProperty("devices")
                            .EnumerateArray()
                            .Select(a => a.GetProperty("serialNumber").GetString() ?? string.Empty)
                            .ToList();

                var query = string.Concat("?device=", string.Join("&device=", sns));

                var sensorUrl = $"https://consumer-api.airthings.com/v1/accounts/{id}/sensors{query}";
                result = await client.GetAsync(sensorUrl);
                json = await result.Content.ReadAsStringAsync();
                _logger.LogInformation(json);
            }
            /*
{"results":[{"serialNumber":"2960166624","sensors":[{"sensorType":"radonShortTermAvg","value":150.0,"unit":"bq"},{"sensorType":"humidity","value":47.0,"unit":"pct"},{"sensorType":"temp","value":16.7,"unit":"c"},{"sensorType":"co2","value":416.0,"unit":"ppm"},{"sensorType":"voc","value":144.0,"unit":"ppb"},{"sensorType":"pressure","value":1009.6,"unit":"mbar"},{"sensorType":"pm25","value":1.0,"unit":"mgpc"},{"sensorType":"pm1","value":1.0,"unit":"mgpc"}],"recorded":"2026-05-12T17:56:56","batteryPercentage":84},{"serialNumber":"2989037410","sensors":[{"sensorType":"radonShortTermAvg","value":118.0,"unit":"bq"},{"sensorType":"humidity","value":41.0,"unit":"pct"},{"sensorType":"temp","value":20.4,"unit":"c"}],"recorded":"2026-05-12T18:02:00","batteryPercentage":96}],"hasNext":false,"totalPages":1}             
             */


            return Results.Ok(AirthingsResponse.Success(new ServiceVersion()));
        });

    private async Task<IResult> HandleApiRequest(HttpContext context, string apiKey, string callerName, Func<Task<IResult>> onAuthorized)
    {
        LogApiRequest(context, callerName);
        if (apiKey == _apiKey)
        {
            return await onAuthorized();
        }
        _logger.LogError("AirthingsService, {0}, {1}, {2}, INVALID API KEY", callerName, context.Connection.RemoteIpAddress, apiKey);
        return Results.NotFound();
    }

    private void LogApiRequest(HttpContext context, [CallerMemberName] string callerName = "")
    {
        _logger.LogInformation("AirthingsService, {0}, {1}",
            callerName, context.Connection.RemoteIpAddress);
    }

    private void LogException(HttpContext context, Exception ex, [CallerMemberName] string callerName = "")
    {
        _logger.LogError(ex, "AirthingsService, {0}, {1}",
            callerName, context.Connection.RemoteIpAddress);
    }

}
