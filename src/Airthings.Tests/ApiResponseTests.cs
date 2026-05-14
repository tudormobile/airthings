namespace Airthings.Tests;

[TestClass]
public class ApiResponseTests
{
    [TestMethod]
    public void ApiResponse_CanBeInstantiated()
    {
        // Act
        var response = new ApiResponse();

        // Assert
        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccess);
        Assert.IsNull(response.Message);
    }

    [TestMethod]
    public void ApiResponse_PropertiesCanBeSet()
    {
        // Arrange & Act
        var message = "Test message";
        var response = new ApiResponse() { Message = message };

        // Assert
        Assert.AreEqual(message, response.Message);
    }

    [TestMethod]
    public void ApiResponse_IsSuccess_ReturnsTrueWhenMessageIsNull()
    {
        // Arrange
        var response = new ApiResponse { Message = null };

        // Act & Assert
        Assert.IsTrue(response.IsSuccess);
    }

    [TestMethod]
    public void ApiResponse_IsSuccess_ReturnsFalseWhenMessageIsSet()
    {
        // Arrange
        var response = new ApiResponse { Message = "Error occurred" };

        // Act & Assert
        Assert.IsFalse(response.IsSuccess);
    }
}
