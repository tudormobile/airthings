using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Tudormobile.Airthings;

/// <summary>
/// Provides a client for interacting with the Airthings Consumer API.
/// </summary>
/// <remarks>Automatically refreshes OAuth2 access tokens when requests receive unauthorized responses. Token
/// refresh operations are thread-safe.</remarks>
public class AirthingsClient : IAirthingsClient
{
    private static readonly string BASE_URL = "https://consumer-api.airthings.com/v1";
    private static readonly string TOKEN_URL = "https://accounts-api.airthings.com/v1/token";

    private readonly HttpClient _httpClient;
    private readonly string _clientId;
    private readonly string _clientSecret;
    private string _accessToken;
    private readonly SemaphoreSlim _tokenRefreshLock = new(1, 1);
    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true,
    };

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
    /// tokens after successful API calls. 
    /// </remarks>
    public AirthingsClient(HttpClient httpClient, string clientId, string clientSecret, string accessToken = "")
    {
        _httpClient = httpClient;
        _clientId = clientId;
        _clientSecret = clientSecret;
        _accessToken = accessToken;
    }

    /// <inheritdoc/>
    public Task<ApiResponse> GetHealth(CancellationToken cancellationToken = default) => ApiRequest<ApiResponse>("/health", cancellationToken);

    /// <inheritdoc/>
    public Task<AccountsResponse> ListAccounts(CancellationToken cancellationToken = default) => ApiRequest<AccountsResponse>("/accounts", cancellationToken);

    /// <inheritdoc/>
    public Task<DevicesResponse> ListDevices(string accountId, CancellationToken cancellationToken = default) => ApiRequest<DevicesResponse>($"/accounts/{accountId}/devices", cancellationToken);

    /// <inheritdoc/>
    public Task<SensorsResponse> ReadSensors(string accountId, IEnumerable<string> serialNumbers, CancellationToken cancellationToken = default)
        => ApiRequest<SensorsResponse>($"/accounts/{accountId}/sensors?device={string.Join("&device=", serialNumbers)}", cancellationToken);

    private async Task<T> ApiRequest<T>(string uriString, CancellationToken cancellationToken) where T : ApiResponse, new()
    {
        uriString = string.Concat(BASE_URL, uriString);
        // Try initial request
        var response = await SendRequestAsync(uriString, cancellationToken);

        // If unauthorized, refresh token and retry once
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            await RefreshTokenAsync(cancellationToken);
            response = await SendRequestAsync(uriString, cancellationToken);
        }

        // Check for success
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
                    scope = new string[] { "read:device:current_values" }
                })
            };

            var tokenResponse = await _httpClient.SendAsync(tokenRequest, cancellationToken);
            tokenResponse.EnsureSuccessStatusCode();

            var tokenJson = await tokenResponse.Content.ReadAsStringAsync(cancellationToken);
            using var doc = JsonDocument.Parse(tokenJson);
            var token = doc.RootElement.GetProperty("access_token").GetString();
            if (token != null)
            {
                _accessToken = token;
            }
            else
            {
                throw new InvalidOperationException("Token refresh failed: No access token returned");
            }
        }
        finally
        {
            _tokenRefreshLock.Release();
        }
    }
}
