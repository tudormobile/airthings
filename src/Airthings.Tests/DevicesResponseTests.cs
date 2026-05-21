namespace Airthings.Tests;

[TestClass]
public class DevicesResponseTests
{
    [TestMethod]
    public void DevicesResponse_CanBeInstantiated()
    {
        // Act
        var response = new DevicesResponse();

        // Assert
        Assert.IsNotNull(response);
        Assert.IsEmpty(response.Devices);
        Assert.IsInstanceOfType<ApiResponse>(response);
    }

    [TestMethod]
    public void DevicesResponse_PropertiesCanBeSet()
    {
        // Arrange
        var response = new DevicesResponse();
        var devices = new List<Device>
        {
            new() { SerialNumber = "123", Name = "Device 1" },
            new() { SerialNumber = "456", Name = "Device 2" }
        };

        // Act
        response.Devices = devices;

        // Assert
        Assert.HasCount(2, response.Devices);
        Assert.AreEqual("123", response.Devices[0].SerialNumber);
        Assert.AreEqual("Device 1", response.Devices[0].Name);
        Assert.AreEqual("456", response.Devices[1].SerialNumber);
        Assert.AreEqual("Device 2", response.Devices[1].Name);
    }
}
