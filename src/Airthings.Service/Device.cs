namespace Tudormobile.Airthings.Service;

/// <summary>
/// Represents an Airthings device with its basic information.
/// </summary>
public sealed record Device
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
}
