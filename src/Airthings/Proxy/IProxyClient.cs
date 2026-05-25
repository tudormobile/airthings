namespace Tudormobile.Airthings.Proxy;

/// <summary>
/// Defines a client for interacting with the Airthings Proxy Service.
/// </summary>
public interface IProxyClient
{
    /// <summary>
    /// Creates a new instance of the Airthings Proxy Client.
    /// </summary>
    /// <param name="apiKey">The API key for authenticating with the proxy service.</param>
    /// <param name="baseAddress">The base address of the proxy service. Must be an absolute URI.</param>
    /// <param name="httpClient">The HTTP client used for API requests.</param>
    /// <returns>A new instance of <see cref="IProxyClient"/>.</returns>
    /// <remarks>
    /// The caller is responsible for disposing of the <paramref name="httpClient"/> instance.
    /// The <paramref name="baseAddress"/> must be an absolute URI (e.g., "https://example.com/api/").
    /// </remarks>
    public static IProxyClient Create(string apiKey, string baseAddress, HttpClient httpClient) => new ProxyClient(apiKey, baseAddress, httpClient);

    /// <summary>
    /// Reads the status information from the Airthings Proxy Service.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="ProxyResponse"/> containing the status information.</returns>
    public Task<ProxyResponse> ReadStatus(CancellationToken cancellationToken = default);

    /// <summary>
    /// Reads the summary of sensor data from the Airthings Proxy Service.
    /// </summary>
    /// <param name="unitsType">The unit system to use for the response data. Defaults to <see cref="UnitsType.Metric"/>.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="ProxyResponse"/> containing the sensor data summary.</returns>
    public Task<ProxyResponse> ReadSummary(UnitsType unitsType = UnitsType.Metric, CancellationToken cancellationToken = default);

}
