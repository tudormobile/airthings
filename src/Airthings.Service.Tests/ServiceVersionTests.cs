namespace AirthingsService.Tests;

[TestClass]
public class ServiceVersionTests
{
    [TestMethod]
    public void Construct_SetsDefaultValues()
    {
        var version = new ServiceVersion();

        Assert.IsFalse(string.IsNullOrWhiteSpace(version.Name));
        Assert.IsFalse(string.IsNullOrWhiteSpace(version.Description));
        Assert.IsFalse(string.IsNullOrWhiteSpace(version.Copyright));
        Assert.IsFalse(string.IsNullOrWhiteSpace(version.Version));
    }
}