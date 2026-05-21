namespace Airthings.Tests;

[TestClass]
public class DeviceTests
{
    [TestMethod]
    public void Device_CanBeInstantiated()
    {
        // Act
        var response = new Device();

        // Assert
        Assert.IsNotNull(response);
        Assert.IsNotNull(response.Type);
        Assert.IsNotNull(response.Name);
        Assert.IsNotNull(response.Home);
        Assert.IsNotNull(response.SerialNumber);
        Assert.IsEmpty(response.Sensors);
    }

    [TestMethod]
    public void Device_PropertiesCanBeSet()
    {
        // Arrange & Act
        var response = new Device
        {
            SerialNumber = "67890",
            Home = "Kitchen",
            Name = "Kitchen Sensor",
            Type = "Wave Mini",
            Sensors = new List<string> { "temp", "humidity" }
        };

        // Assert
        Assert.AreEqual("67890", response.SerialNumber);
        Assert.AreEqual("Kitchen", response.Home);
        Assert.AreEqual("Kitchen Sensor", response.Name);
        Assert.AreEqual("Wave Mini", response.Type);
        Assert.HasCount(2, response.Sensors);
    }
}
