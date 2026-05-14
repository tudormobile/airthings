namespace Airthings.IntegrationTests;

[TestClass]
public class SimpleTest
{
    [TestMethod]
    public void ErrorResponseTest()
    {
        // We need at least 1 simple test to avoid MS Test failing on "Zero tests run" error in our CI/CD pipeline.
        var response = new ApiResponse();
        Assert.IsTrue(response.IsSuccess);
    }
}
