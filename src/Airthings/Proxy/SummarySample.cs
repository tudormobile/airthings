namespace Tudormobile.Airthings.Proxy;

/// <summary>
/// Represents a summary of samples for a specific device.
/// </summary>
public sealed record SummarySample
{
    /// <summary>
    /// Gets or sets the name of the home or location the device is assigned to.
    /// </summary>
    public string Home { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name of the device.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the latest radon short-term average level measured by the device.
    /// </summary>
    public double Radon { get; init; }

    /// <summary>
    /// Gets or sets the latest humidity level measured by the device.
    /// </summary>
    public double Humidity { get; init; }

    /// <summary>
    /// Gets or sets the latest temperature measured by the device.
    /// </summary>
    public double Temperature { get; init; }

    /// <summary>
    /// Gets or sets the date and time when the event was recorded by the device.
    /// </summary>
    public DateTime Recorded { get; set; }

    /// <summary>
    /// Gets or sets the battery percentage of the device.
    /// </summary>
    public int BatteryPercentage { get; set; }

}
