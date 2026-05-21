
namespace Tudormobile.Airthings.Service;

/// <summary>
/// Provides factory methods for building <see cref="SummarySample"/> instances from raw Airthings API data.
/// </summary>
public static class SummaryBuilder
{
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
            Home = device?.Home ?? "Unknown",
            Name = device?.Name ?? "Unknown",
            Radon = r.Sensors.FirstOrDefault(s => s.SensorType == "radonShortTermAvg")?.Value ?? 0,
            Humidity = r.Sensors.FirstOrDefault(s => s.SensorType == "humidity")?.Value ?? 0,
            Temperature = r.Sensors.FirstOrDefault(s => s.SensorType == "temp")?.Value ?? 0,
        };
    }
}
