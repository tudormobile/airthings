namespace Tudormobile.Airthings;

/// <summary>
/// Represents the response from a devices samples request.
/// </summary>
/// <remarks>
/// Mapping of the Airthings API response.
/// </remarks>
public record DevicesSamplesResponse : ApiResponse
{
    /// <summary>
    /// Gets or sets the collection of sensor readings, one per requested device.
    /// </summary>
    public List<SensorsResponse> Results { get; set; } = [];

    /// <summary>
    /// Gets or sets a value indicating whether additional pages of results are available.
    /// </summary>
    public bool HasNext { get; set; }

    /// <summary>
    /// Gets or sets the total number of pages available for this request.
    /// </summary>
    public int TotalPages { get; set; }
}
