namespace Tudormobile.Airthings;

public class SensorResponse
{
    public string SensorType { get; set; } = string.Empty;
    public double Value { get; set; }
    public string Unit { get; set; } = string.Empty;
}
/* Example:
{
    "sensorType":"radonShortTermAvg",
    "value":150.0,
    "unit":"bq"
}
*/