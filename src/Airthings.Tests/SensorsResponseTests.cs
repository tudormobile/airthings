namespace Airthings.Tests;

[TestClass]
public class SensorsResponseTests
{
    [TestMethod]
    public void SensorsResponse_CanBeInstantiated()
    {
        // Act
        var response = new SensorsResponse();

        // Assert
        Assert.IsNotNull(response);
        Assert.IsEmpty(response.Sensors);
        Assert.IsNotNull(response.SerialNumber);
    }

    [TestMethod]
    public void SensorsResponse_PropertiesCanBeSet()
    {
        // Arrange
        var serialNumber = "2960166624";
        var sensors = new List<Sensor>
        {
            new() { SensorType = "radonShortTermAvg", Value = 150.0, Unit = "bq" }
        };
        var recordedDate = new DateTime(2026, 5, 12, 17, 56, 56);
        var batteryPercentage = 84;

        // Act
        var response = new SensorsResponse
        {
            SerialNumber = serialNumber,
            Sensors = sensors,
            Recorded = recordedDate,
            BatteryPercentage = batteryPercentage
        };

        // Assert
        Assert.AreEqual(serialNumber, response.SerialNumber);
        Assert.HasCount(1, response.Sensors);
        Assert.AreEqual(recordedDate, response.Recorded);
        Assert.AreEqual(batteryPercentage, response.BatteryPercentage);
    }
}
