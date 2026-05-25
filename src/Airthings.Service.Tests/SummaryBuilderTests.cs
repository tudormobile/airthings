namespace AirthingsService.Tests;

[TestClass]
public class SummaryBuilderTests
{
    private static readonly Dictionary<string, Tudormobile.Airthings.Device> _deviceMapping = new()
    {
        ["123456"] = new Tudormobile.Airthings.Device { Home = "Basement", Name = "Wave Plus", SerialNumber = "123456" },
        ["789012"] = new Tudormobile.Airthings.Device { Home = "Living Room", Name = "Wave Mini", SerialNumber = "789012" },
    };

    private static SensorsResponse BuildSensors(string serial, double radon = 0, double humidity = 0, double temp = 0)
        => new()
        {
            SerialNumber = serial,
            Sensors =
            [
                new Sensor { SensorType = "radonShortTermAvg", Value = radon },
                new Sensor { SensorType = "humidity", Value = humidity },
                new Sensor { SensorType = "temp", Value = temp },
            ]
        };

    [TestMethod]
    public void CreateSummary_MapsHomeAndName_FromDeviceMapping()
    {
        var sensors = BuildSensors("123456");

        var result = SummaryBuilder.CreateSummary(_deviceMapping, sensors);

        Assert.AreEqual("Basement", result.Home);
        Assert.AreEqual("Wave Plus", result.Name);
    }

    [TestMethod]
    public void CreateSummary_MapsSensorValues()
    {
        var sensors = BuildSensors("123456", radon: 150.0, humidity: 45.5, temp: 21.3);

        var result = SummaryBuilder.CreateSummary(_deviceMapping, sensors);

        Assert.AreEqual(150.0, result.Radon);
        Assert.AreEqual(45.5, result.Humidity);
        Assert.AreEqual(21.3, result.Temperature);
    }

    [TestMethod]
    public void CreateSummary_UnknownSerial_DefaultsHomeAndName()
    {
        var sensors = BuildSensors("UNKNOWN");

        var result = SummaryBuilder.CreateSummary(_deviceMapping, sensors);

        Assert.AreEqual("Unknown", result.Home);
        Assert.AreEqual("Unknown", result.Name);
    }

    [TestMethod]
    public void CreateSummary_MissingSensor_DefaultsToZero()
    {
        var sensors = new SensorsResponse
        {
            SerialNumber = "123456",
            Sensors = [] // no sensor readings at all
        };

        var result = SummaryBuilder.CreateSummary(_deviceMapping, sensors);

        Assert.AreEqual(0, result.Radon);
        Assert.AreEqual(0, result.Humidity);
        Assert.AreEqual(0, result.Temperature);
        Assert.AreEqual(0, result.BatteryPercentage);
    }

    [TestMethod]
    public void CreateSummary_EmptyDeviceMapping_DefaultsHomeAndName()
    {
        var sensors = BuildSensors("123456", radon: 100.0);

        var result = SummaryBuilder.CreateSummary(new Dictionary<string, Tudormobile.Airthings.Device>(), sensors);

        Assert.AreEqual("Unknown", result.Home);
        Assert.AreEqual("Unknown", result.Name);
        Assert.AreEqual(100.0, result.Radon);
    }

    [TestMethod]
    public void CreateSummary_PartialSensors_UnmatchedSensorsDefaultToZero()
    {
        var recorded = DateTime.Now;
        var sensors = new SensorsResponse
        {
            BatteryPercentage = 12,
            Recorded = recorded,
            SerialNumber = "789012",
            Sensors =
            [
                new Sensor { SensorType = "humidity", Value = 60.0 },
                // radon and temp intentionally omitted
            ]
        };

        var result = SummaryBuilder.CreateSummary(_deviceMapping, sensors);

        Assert.AreEqual(60.0, result.Humidity);
        Assert.AreEqual(0, result.Radon);
        Assert.AreEqual(0, result.Temperature);
        Assert.AreEqual(12, result.BatteryPercentage);
        Assert.AreEqual(recorded, result.Recorded);
    }
}
