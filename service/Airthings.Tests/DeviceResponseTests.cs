namespace Airthings.Tests;

[TestClass]
public class DeviceResponseTests
{
    [TestMethod]
    public void DeviceResponse_CanBeInstantiated()
    {
        // Act
        var response = new DeviceResponse();

        // Assert
        Assert.IsNotNull(response);
        Assert.IsNotNull(response.Type);
        Assert.IsNotNull(response.Name);
        Assert.IsNotNull(response.Home);
        Assert.IsNotNull(response.SerialNumber);
        Assert.IsEmpty(response.Sensors);
    }

    [TestMethod]
    public void DeviceResponse_PropertiesCanBeSet()
    {
        // Arrange & Act
        var response = new DeviceResponse
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
