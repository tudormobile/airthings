namespace Tudormobile.Airthings.Proxy;

/// <summary>
/// Represents a response from the proxy service containing all summary data retrieved through the proxy service from the Airthings API.
/// </summary>
public record ProxyResponse : ApiResponse
{
    /// <summary>
    /// Gets or sets the UTC timestamp indicating when this summary was last populated from the Airthings API.
    /// </summary>
    public DateTimeOffset LastUpdated { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets the proxy service version.
    /// </summary>
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the collection of individual sensor summaries, one per device.
    /// </summary>
    public List<SummarySample> Samples { get; set; } = [];

}
