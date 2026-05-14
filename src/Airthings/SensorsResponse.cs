namespace Tudormobile.Airthings;

/// <summary>
/// Represents the response from a sensors request for a specific device.
/// </summary>
public record SensorsResponse
{
    /// <summary>
    /// Associated device serial number. This is the unique identifier for the device, 
    /// and can be used to correlate sensor data with devices from the devices endpoint.
    /// </summary>
    public string SerialNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the collection of sensor responses associated with the current device.
    /// </summary>
    public List<SensorResponse> Sensors { get; set; } = [];

    /// <summary>
    /// Gets or sets the date and time when the event was recorded by the device.
    /// </summary>
    public DateTime Recorded { get; set; }

    /// <summary>
    /// Gets or sets the battery percentage of the device.
    /// </summary>
    public int BatteryPercentage { get; set; }

}
/* Example:
{
    "serialNumber":"2960166624",
    "sensors":[
        {
            "sensorType":"radonShortTermAvg",
            "value":150.0,
            "unit":"bq"
        }
    ],
    "recorded":"2026-05-12T17:56:56",
    "batteryPercentage":84
}
*/