namespace Airthings.Tests;

[TestClass]
public class AccountsResponseTests
{
    [TestMethod]
    public void AccountsResponse_CanBeInstantiated()
    {
        // Act
        var response = new AccountsResponse();

        // Assert
        Assert.IsNotNull(response);
        Assert.IsInstanceOfType<ApiResponse>(response);
        Assert.IsEmpty(response.Accounts);
    }

    [TestMethod]
    public void AccountsResponse_PropertiesCanBeSet()
    {
        // Arrange
        var response = new AccountsResponse();
        var accounts = new List<Account>
        {
            new() { Id = "account-1" },
            new() { Id = "account-2" }
        };

        // Act
        response.Accounts = accounts;

        // Assert
        Assert.HasCount(2, response.Accounts);
        Assert.AreEqual("account-1", response.Accounts[0].Id);
        Assert.AreEqual("account-2", response.Accounts[1].Id);
    }

}
