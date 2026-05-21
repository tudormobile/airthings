namespace AirthingsService.Tests;

[TestClass]
public class AirthingsOptionsTests
{
    [TestMethod]
    public void Constructor_DefaultsToEmptyStrings()
    {
        var options = new AirthingsOptions();

        Assert.AreEqual(string.Empty, options.ApiKey);
        Assert.AreEqual(string.Empty, options.ClientId);
        Assert.AreEqual(string.Empty, options.ClientSecret);
    }

    [TestMethod]
    public void Properties_CanBeSet()
    {
        var options = new AirthingsOptions
        {
            ApiKey = "key",
            ClientId = "id",
            ClientSecret = "secret"
        };

        Assert.AreEqual("key", options.ApiKey);
        Assert.AreEqual("id", options.ClientId);
        Assert.AreEqual("secret", options.ClientSecret);
    }
}
