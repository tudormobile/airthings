namespace Tudormobile.Airthings;

/// <summary>
/// Represents a single sensor reading from an Airthings device.
/// </summary>
/// <remarks>
/// Mapping of the Airthings API response.
/// </remarks>
public record Sensor
{
    /// <summary>
    /// Gets or sets the type of sensor (e.g., <c>radonShortTermAvg</c>).
    /// </summary>
    public string SensorType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the sensor reading value.
    /// </summary>
    public double Value { get; set; }

    /// <summary>
    /// Gets or sets the unit of measurement for the reading (e.g., <c>bq</c>).
    /// </summary>
    public string Unit { get; set; } = string.Empty;
}
/* Example:
{
    "sensorType":"radonShortTermAvg",
    "value":150.0,
    "unit":"bq"
}
*/