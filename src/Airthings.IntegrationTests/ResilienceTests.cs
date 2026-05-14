namespace Airthings.IntegrationTests;

[TestClass]
[TestCategory("Integration")]
[TestCategory("Performance")]
public class ResilienceTests
{
    private static AirthingsClient Client => TestFixture.SharedClient;
    public TestContext TestContext { get; set; } // MSTest will set this property

    [TestMethod]
    [TestCategory("Stress")]
    public async Task MultipleSequentialRequests_AllSucceed()
    {
        // Arrange
        var results = new List<bool>();

        // Act - Make 5 requests with proper rate limiting
        for (int i = 0; i < 5; i++)
        {
            if (i > 0) await Task.Delay(5000, TestContext.CancellationToken); // Rate limit
            var result = await Client.ListAccounts(cancellationToken: TestContext.CancellationToken);
            results.Add(result.IsSuccess);
        }

        // Assert
        Assert.IsTrue(results.All(r => r), "All requests should succeed");
    }

    [TestMethod]
    [TestCategory("Cancellation")]
    public async Task ListAccounts_RespectsCancellationToken()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        cts.Cancel(); // Cancel before the call so it is guaranteed to be seen

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => Client.ListAccounts(cancellationToken: cts.Token));
    }
}