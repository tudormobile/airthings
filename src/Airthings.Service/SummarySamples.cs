namespace Tudormobile.Airthings.Service;

/// <summary>
/// Represents a summary of the latest sensor readings across all devices, including service metadata.
/// </summary>
public record SummarySamples
{
    /// <summary>
    /// Gets or sets the UTC timestamp indicating when this summary was last populated from the Airthings API.
    /// </summary>
    public DateTimeOffset LastUpdated { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets the version and metadata of the Airthings Service that produced this response.
    /// </summary>
    public ServiceVersion Version { get; set; } = new();

    /// <summary>
    /// Gets or sets the collection of individual sensor summaries, one per device.
    /// </summary>
    public List<SummarySample> Samples { get; set; } = [];
}
