namespace Tudormobile.Airthings.Service;

/// <summary>
/// Represents a sensor reading within an Airthings device.
/// </summary>
public sealed record DeviceSample
{
    /// <summary>
    /// Gets or sets the type of the sensor.
    /// </summary>
    public string SensorType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the sensor reading value.
    /// </summary>
    public double Value { get; set; }
}
