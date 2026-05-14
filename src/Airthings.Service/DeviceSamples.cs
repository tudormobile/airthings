namespace Tudormobile.Airthings.Service;

/// <summary>
/// Represents a sensor within an Airthings device.
/// </summary>
public sealed record DeviceSamples
{
    /// <summary>
    /// Associated device serial number. This is the unique identifier for the device, 
    /// and can be used to correlate sensor data with devices from the devices endpoint.
    /// </summary>
    public string SerialNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the collection of sensor responses associated with the current device.
    /// </summary>
    public List<DeviceSample> Samples { get; set; } = [];

    /// <summary>
    /// Gets or sets the date and time when the event was recorded by the device.
    /// </summary>
    public DateTime Recorded { get; set; }

    /// <summary>
    /// Gets or sets the battery percentage of the device.
    /// </summary>
    public int BatteryPercentage { get; set; }
}
