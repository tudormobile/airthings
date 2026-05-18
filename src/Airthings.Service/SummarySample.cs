namespace Tudormobile.Airthings.Service;

/// <summary>
/// Represents a summary of samples for a specific device.
/// </summary>
public class SummarySample
{
    /// <summary>
    /// Gets or sets the name of the home or location the device is assigned to.
    /// </summary>
    public string Home { get; internal set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name of the device.
    /// </summary>
    public string Name { get; internal set; } = string.Empty;

    /// <summary>
    /// Gets or sets the latest radon short-term average level measured by the device.
    /// </summary>
    public double Radon { get; internal set; }

    /// <summary>
    /// Gets or sets the latest humidity level measured by the device.
    /// </summary>
    public double Humidity { get; internal set; }

    /// <summary>
    /// Gets or sets the latest temperature measured by the device.
    /// </summary>
    public double Temperature { get; internal set; }
}
