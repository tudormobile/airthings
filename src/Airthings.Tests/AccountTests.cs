namespace Airthings.Tests;

[TestClass]
public class AccountTests
{
    [TestMethod]
    public void Account_CanBeInstantiated()
    {
        // Act
        var response = new Account();

        // Assert
        Assert.IsNotNull(response);
        Assert.IsNotNull(response.Id);
    }

    [TestMethod]
    public void Account_PropertiesCanBeSet()
    {
        // Arrange
        var id = "account-456";

        // Act
        var response = new Account { Id = id };

        // Assert
        Assert.AreEqual(id, response.Id);
    }
}
