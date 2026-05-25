using System.Text.Json;

namespace Tudormobile.Airthings.Proxy;

/// <inheritdoc/>
internal class ProxyClient : IProxyClient
{
    private const string PROXY_SERVICE_PATH = "/home/airthings/v1";
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private string _apiKey;
    private HttpClient _httpClient;
    private Uri _baseUri;

    /// <inheritdoc/>
    public ProxyClient(string apiKey, string baseAddress, HttpClient httpClient)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(apiKey);
        ArgumentNullException.ThrowIfNullOrWhiteSpace(baseAddress);
        ArgumentNullException.ThrowIfNull(httpClient);
        _apiKey = apiKey;
        _httpClient = httpClient;

        // Validate the provided Uri string
        if (!Uri.IsWellFormedUriString(baseAddress, UriKind.Absolute))
        {
            throw new ArgumentException("Must be well-formed absolute Uri string.", nameof(baseAddress));
        }

        var builder = new UriBuilder(baseAddress);
        builder.Path = PROXY_SERVICE_PATH;
        _baseUri = builder.Uri;
    }

    public Task<ProxyResponse> ReadStatus(CancellationToken cancellationToken = default)
        => ApiRequest("status", cancellationToken);

    public Task<ProxyResponse> ReadSummary(UnitsType unitsType = UnitsType.Metric, CancellationToken cancellationToken = default)
        => ApiRequest($"summary/{unitsType.ToString().ToLowerInvariant()}", cancellationToken);

    private async Task<ProxyResponse> ApiRequest<T>(string uriString, CancellationToken cancellationToken)
    {
        uriString = new Uri(_baseUri, uriString).ToString();
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, uriString);
            request.Headers.Add("ApiKey", _apiKey);
            using var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;
                var isSuccess = root.GetProperty("isSuccess").GetBoolean();
                if (!isSuccess)
                {
                    var message = root.GetProperty("data").GetString();
                    return new ProxyResponse() { Message = message };
                }
                // could just be version
                var versionProperty = root.GetProperty("data").GetProperty("version");
                if (versionProperty.ValueKind == JsonValueKind.String)
                {
                    return new ProxyResponse() { Version = versionProperty.GetString() ?? string.Empty };
                }
                var version = versionProperty.GetProperty("version").GetString();
                var samplesElement = root.GetProperty("data").GetProperty("samples");
                var samples = JsonSerializer.Deserialize<List<SummarySample>>(samplesElement.GetRawText(), JsonOptions) ?? [];
                var result = new ProxyResponse()
                {
                    Version = version ?? string.Empty,
                    Samples = samples
                };

                return result;
            }
            catch (JsonException ex)
            {
                return new ProxyResponse { Message = $"Invalid JSON: {ex.Message}" };
            }

        }
        catch (HttpRequestException ex)
        {
            // Covers network failures, DNS errors, and failed token refresh
            return new ProxyResponse { Message = $"Network error: {ex.Message}" };
        }
    }

}
