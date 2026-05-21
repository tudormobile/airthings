namespace Airthings.Tests;

[TestClass]
public class SensorTests
{
    [TestMethod]
    public void Sensor_CanBeInstantiated()
    {
        // Act
        var response = new Sensor();

        // Assert
        Assert.IsNotNull(response);
        Assert.IsNotNull(response.SensorType);
        Assert.IsNotNull(response.Unit);
        Assert.AreEqual(default, response.Value);
    }

    [TestMethod]
    public void Sensor_PropertiesCanBeSet()
    {
        // Arrange & Act
        var response = new Sensor
        {
            SensorType = "temp",
            Value = 22.5,
            Unit = "c"
        };

        // Assert
        Assert.AreEqual("temp", response.SensorType);
        Assert.AreEqual(22.5, response.Value);
        Assert.AreEqual("c", response.Unit);
    }
}
