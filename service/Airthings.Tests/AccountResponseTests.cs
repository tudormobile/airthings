namespace Airthings.Tests;

[TestClass]
public class AccountResponseTests
{
    [TestMethod]
    public void AccountResponse_CanBeInstantiated()
    {
        // Act
        var response = new AccountResponse();

        // Assert
        Assert.IsNotNull(response);
        Assert.IsNotNull(response.Id);
    }

    [TestMethod]
    public void AccountResponse_PropertiesCanBeSet()
    {
        // Arrange
        var id = "account-456";

        // Act
        var response = new AccountResponse { Id = id };

        // Assert
        Assert.AreEqual(id, response.Id);
    }
}
