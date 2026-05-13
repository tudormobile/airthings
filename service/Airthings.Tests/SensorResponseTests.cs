namespace Airthings.Tests;

[TestClass]
public class SensorResponseTests
{
    [TestMethod]
    public void SensorResponse_CanBeInstantiated()
    {
        // Act
        var response = new SensorResponse();

        // Assert
        Assert.IsNotNull(response);
        Assert.IsNotNull(response.SensorType);
        Assert.IsNotNull(response.Unit);
        Assert.AreEqual(default, response.Value);
    }

    [TestMethod]
    public void SensorResponse_PropertiesCanBeSet()
    {
        // Arrange & Act
        var response = new SensorResponse
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
