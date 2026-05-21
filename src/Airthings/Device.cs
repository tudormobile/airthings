namespace Tudormobile.Airthings;

/// <summary>
/// Represents an Airthings device.
/// </summary>
/// <remarks>
/// Mapping of the Airthings API response.
/// </remarks>
public record Device
{
    /// <summary>
    /// Gets or sets the unique serial number of the device.
    /// </summary>
    public string SerialNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the home or location the device is assigned to.
    /// </summary>
    public string Home { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name of the device.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the device type (e.g., <c>WAVE_PLUS</c>).
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the collection of sensor identifiers available on this device.
    /// </summary>
    public List<string> Sensors { get; set; } = [];
}
