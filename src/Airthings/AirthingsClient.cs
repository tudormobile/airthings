using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Tudormobile.Airthings;

/// <summary>
/// Provides a client for interacting with the Airthings Consumer API.
/// </summary>
/// <remarks>
/// <para>
/// Automatically refreshes OAuth2 access tokens when requests receive unauthorized responses.
/// Token refresh operations are serialized via a semaphore, but no double-check guard is applied.
/// Under concurrent load, two callers may each trigger a sequential refresh. This is acceptable
/// when an output cache limits upstream calls to a low frequency (e.g., once per 30–60 minutes).
/// </para>
/// <para>
/// <b>Design constraints:</b> This client is bound to a single <c>ClientId</c> / <c>ClientSecret</c>
/// pair supplied at construction time. It does not support per-request credentials and is therefore
/// suited only for single-account, single-user deployments. Pagination of multi-page API responses
/// is not implemented; callers that expect large result sets must account for this limitation.
/// </para>
/// </remarks>
public sealed class AirthingsClient : IAirthingsClient
{
    // Airthings API endpoints
    private static readonly string BASE_URL = "https://consumer-api.airthings.com/v1";
    private static readonly string TOKEN_URL = "https://accounts-api.airthings.com/v1/token";

    // Airthings API scope for reading sensor data. Deviates from standard client credentials scope format,
    // but matches Airthings API documentation and examples.
    private static readonly string[] AIRTHINGS_API_SCOPE = ["read:device:current_values"];

    private readonly HttpClient _httpClient;
    private readonly string _clientId;
    private readonly string _clientSecret;
    private string _accessToken;
    private readonly SemaphoreSlim _tokenRefreshLock = new(1, 1);
    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    /// <inheritdoc/>
    public void Dispose() => _tokenRefreshLock.Dispose();

    /// <summary>
    /// Gets the access token, which was possibly refreshed.
    /// </summary>
    public string AccessToken => _accessToken;

    /// <summary>
    /// Initializes a new instance of the <see cref="AirthingsClient"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client used for API requests. Callers are responsible to dispose of this instance.</param>
    /// <param name="clientId">The client ID for authentication.</param>
    /// <param name="clientSecret">The client secret for authentication.</param>
    /// <param name="accessToken">The optional access token. Defaults to an empty string.</param>
    /// <remarks>
    /// Callers can provide an optional access token from previous sessions, and also read and store refreshed
    /// tokens after successful API calls. Callers are responsible to managing the lifecycle of the provided <see cref="HttpClient"/> instance, including disposal. 
    /// The client will not dispose of the provided <see cref="HttpClient"/> instance.
    /// </remarks>
    public AirthingsClient(HttpClient httpClient, string clientId, string clientSecret, string accessToken = "")
    {
        _httpClient = httpClient;
        _clientId = clientId;
        _clientSecret = clientSecret;
        _accessToken = accessToken;
    }

    /// <inheritdoc/>
    public Task<AccountsResponse> ListAccounts(CancellationToken cancellationToken = default) => ApiRequest<AccountsResponse>("/accounts", cancellationToken);

    /// <inheritdoc/>
    public Task<DevicesResponse> ListDevices(string accountId, CancellationToken cancellationToken = default) => ApiRequest<DevicesResponse>($"/accounts/{accountId}/devices", cancellationToken);

    /// <inheritdoc/>
    public Task<DevicesSamplesResponse> ReadSensors(string accountId, IEnumerable<string> serialNumbers, UnitsType unitsType = UnitsType.Metric, CancellationToken cancellationToken = default)
        => ApiRequest<DevicesSamplesResponse>($"/accounts/{accountId}/sensors?device={string.Join("&device=", serialNumbers)}&unit={unitsType.ToString().ToLower()}", cancellationToken);

    private async Task<T> ApiRequest<T>(string uriString, CancellationToken cancellationToken) where T : ApiResponse, new()
    {
        uriString = string.Concat(BASE_URL, uriString);
        HttpResponseMessage response;
        try
        {
            // Try initial request
            response = await SendRequestAsync(uriString, cancellationToken);

            // If unauthorized, refresh token and retry once
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await RefreshTokenAsync(cancellationToken);
                response = await SendRequestAsync(uriString, cancellationToken);
            }
        }
        catch (HttpRequestException ex)
        {
            // Covers network failures, DNS errors, and failed token refresh
            return new T { Message = $"Network error: {ex.Message}" };
        }

        // Check for non-success HTTP status
        if (!response.IsSuccessStatusCode)
        {
            return new T { Message = $"Request failed with status {(int)response.StatusCode}: {response.ReasonPhrase}" };
        }

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        try
        {
            var result = string.IsNullOrEmpty(json) ? new T() : JsonSerializer.Deserialize<T>(json, _options) ?? new T { Message = "Request failed." };
            return result;
        }
        catch (JsonException ex)
        {
            return new T { Message = $"Invalid JSON: {ex.Message}" };
        }
    }

    private async Task<HttpResponseMessage> SendRequestAsync(string uriString, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, uriString);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        return await _httpClient.SendAsync(request, cancellationToken);
    }

    private async Task RefreshTokenAsync(CancellationToken cancellationToken)
    {
        // Use semaphore to ensure only one token refresh happens at a time
        await _tokenRefreshLock.WaitAsync(cancellationToken);
        try
        {
            // OAuth2 client credentials request
            var tokenRequest = new HttpRequestMessage(HttpMethod.Post, TOKEN_URL)
            {
                Content = JsonContent.Create(new
                {
                    grant_type = "client_credentials",
                    client_id = _clientId,
                    client_secret = _clientSecret,
                    scope = AIRTHINGS_API_SCOPE
                })
            };

            var tokenResponse = await _httpClient.SendAsync(tokenRequest, cancellationToken);
            tokenResponse.EnsureSuccessStatusCode();

            var tokenJson = await tokenResponse.Content.ReadAsStringAsync(cancellationToken);
            try
            {
                using var doc = JsonDocument.Parse(tokenJson);
                if (!doc.RootElement.TryGetProperty("access_token", out var tokenElement) ||
                    tokenElement.GetString() is not string token)
                {
                    throw new HttpRequestException("Token refresh failed: No access token in response.");
                }
                _accessToken = token;
            }
            catch (JsonException ex)
            {
                throw new HttpRequestException($"Token refresh failed: Malformed response. {ex.Message}", ex);
            }
        }
        finally
        {
            _tokenRefreshLock.Release();
        }
    }
}
