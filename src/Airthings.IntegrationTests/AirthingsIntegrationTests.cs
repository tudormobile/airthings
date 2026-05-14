namespace Airthings.IntegrationTests;

[TestClass]
[TestCategory("Integration")]
public class AirthingsIntegrationTests
{
    private static AirthingsClient Client => TestFixture.SharedClient;
    public TestContext TestContext { get; set; } // MSTest will set this property

    [TestMethod]
    [TestCategory("RealApi")]
    public async Task ListAccounts_ReturnsValidTokenAndAccounts()
    {
        // Act
        var result = await Client.ListAccounts(TestContext.CancellationToken);
        // Assert
        Assert.IsNotEmpty(result.Accounts);
        Assert.IsTrue(result.IsSuccess);
        Assert.IsFalse(string.IsNullOrWhiteSpace(((AirthingsClient)Client).AccessToken));
    }
}