namespace Tudormobile.Airthings;

public class SensorsResponse : ApiResponse
{
    public string SerialNumber { get; set; } = string.Empty;
    public List<SensorResponse> Sensors { get; set; } = [];
    public DateTime Recorded { get; set; }
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