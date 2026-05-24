
namespace Tudormobile.Airthings.Service;

/// <summary>
/// Provides factory methods for building <see cref="SummarySample"/> instances from raw Airthings API data.
/// </summary>
public static class SummaryBuilder
{
    private const string UNKNOWN_NAME = "Unknown";
    private const string RADON_SENSOR_KEY = "radonShortTermAvg";
    private const string HUMIDITY_SENSOR_KEY = "humidity";
    private const string TEMPERATURE_SENSOR_KEY = "temp";

    /// <summary>
    /// Creates a <see cref="SummarySample"/> by combining device metadata with its latest sensor readings.
    /// </summary>
    /// <param name="deviceMapping">
    /// A dictionary keyed by device serial number, used to look up the home and display name for each device.
    /// </param>
    /// <param name="r">
    /// The sensor response for a single device, containing the serial number and a list of sensor readings.
    /// </param>
    /// <returns>
    /// A <see cref="SummarySample"/> populated with the device's home, name, radon level, humidity, and temperature.
    /// Sensor values default to <c>0</c> if the corresponding sensor type is not present in the response.
    /// Home and name default to <c>"Unknown"</c> if the serial number is not found in <paramref name="deviceMapping"/>.
    /// </returns>
    internal static SummarySample CreateSummary(IDictionary<string, Airthings.Device> deviceMapping, SensorsResponse r)
    {
        deviceMapping.TryGetValue(r.SerialNumber, out var device);
        return new SummarySample
        {
            Home = device?.Home ?? UNKNOWN_NAME,
            Name = device?.Name ?? UNKNOWN_NAME,
            Radon = r.Sensors.FirstOrDefault(s => s.SensorType == RADON_SENSOR_KEY)?.Value ?? 0,
            Humidity = r.Sensors.FirstOrDefault(s => s.SensorType == HUMIDITY_SENSOR_KEY)?.Value ?? 0,
            Temperature = r.Sensors.FirstOrDefault(s => s.SensorType == TEMPERATURE_SENSOR_KEY)?.Value ?? 0,
        };
    }
}
