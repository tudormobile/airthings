namespace Airthings.Tests.Proxy;

[TestClass]
public class IProxyClientTests
{
    [TestMethod]
    public void Create_WithValidParameters_ReturnsProxyClient()
    {
        // Arrange
        var apiKey = "test-api-key";
        var baseAddress = "https://example.com/api";
        using var httpClient = new HttpClient();

        // Act
        var client = IProxyClient.Create(apiKey, baseAddress, httpClient);

        // Assert
        Assert.IsNotNull(client);
        Assert.IsInstanceOfType<IProxyClient>(client);
    }

    [TestMethod]
    public void Create_WithNullApiKey_ThrowsArgumentNullException()
    {
        // Arrange
        string? apiKey = null;
        var baseAddress = "https://example.com/api";
        using var httpClient = new HttpClient();

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() =>
            IProxyClient.Create(apiKey!, baseAddress, httpClient));
    }

    [TestMethod]
    public void Create_WithEmptyApiKey_ThrowsArgumentException()
    {
        // Arrange
        var apiKey = string.Empty;
        var baseAddress = "https://example.com/api";
        using var httpClient = new HttpClient();

        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() =>
            IProxyClient.Create(apiKey, baseAddress, httpClient));
    }

    [TestMethod]
    public void Create_WithWhitespaceApiKey_ThrowsArgumentException()
    {
        // Arrange
        var apiKey = "   ";
        var baseAddress = "https://example.com/api";
        using var httpClient = new HttpClient();

        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() =>
            IProxyClient.Create(apiKey, baseAddress, httpClient));
    }

    [TestMethod]
    public void Create_WithNullBaseAddress_ThrowsArgumentNullException()
    {
        // Arrange
        var apiKey = "test-api-key";
        string? baseAddress = null;
        using var httpClient = new HttpClient();

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() =>
            IProxyClient.Create(apiKey, baseAddress!, httpClient));
    }

    [TestMethod]
    public void Create_WithEmptyBaseAddress_ThrowsArgumentException()
    {
        // Arrange
        var apiKey = "test-api-key";
        var baseAddress = string.Empty;
        using var httpClient = new HttpClient();

        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() =>
            IProxyClient.Create(apiKey, baseAddress, httpClient));
    }

    [TestMethod]
    public void Create_WithRelativeBaseAddress_ThrowsArgumentException()
    {
        // Arrange
        var apiKey = "test-api-key";
        var baseAddress = "/relative/path";
        using var httpClient = new HttpClient();

        // Act & Assert
        var exception = Assert.ThrowsExactly<ArgumentException>(() =>
            IProxyClient.Create(apiKey, baseAddress, httpClient));
        Assert.IsTrue(exception.Message.Contains("absolute", StringComparison.OrdinalIgnoreCase));
    }

    [TestMethod]
    public void Create_WithInvalidUri_ThrowsArgumentException()
    {
        // Arrange
        var apiKey = "test-api-key";
        var baseAddress = "not a valid uri";
        using var httpClient = new HttpClient();

        // Act & Assert
        var exception = Assert.ThrowsExactly<ArgumentException>(() =>
            IProxyClient.Create(apiKey, baseAddress, httpClient));
        Assert.IsTrue(exception.Message.Contains("absolute", StringComparison.OrdinalIgnoreCase));
    }

    [TestMethod]
    public void Create_WithNullHttpClient_ThrowsArgumentNullException()
    {
        // Arrange
        var apiKey = "test-api-key";
        var baseAddress = "https://example.com/api";
        HttpClient? httpClient = null;

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() =>
            IProxyClient.Create(apiKey, baseAddress, httpClient!));
    }

    [TestMethod]
    public void Create_WithHttpsBaseAddress_ReturnsProxyClient()
    {
        // Arrange
        var apiKey = "test-api-key";
        var baseAddress = "https://secure.example.com:8443/api/v1";
        using var httpClient = new HttpClient();

        // Act
        var client = IProxyClient.Create(apiKey, baseAddress, httpClient);

        // Assert
        Assert.IsNotNull(client);
    }

    [TestMethod]
    public void Create_WithHttpBaseAddress_ReturnsProxyClient()
    {
        // Arrange
        var apiKey = "test-api-key";
        var baseAddress = "http://localhost:5000/api";
        using var httpClient = new HttpClient();

        // Act
        var client = IProxyClient.Create(apiKey, baseAddress, httpClient);

        // Assert
        Assert.IsNotNull(client);
    }

    [TestMethod]
    public void Create_WithTrailingSlashBaseAddress_ReturnsProxyClient()
    {
        // Arrange
        var apiKey = "test-api-key";
        var baseAddress = "https://example.com/api/";
        using var httpClient = new HttpClient();

        // Act
        var client = IProxyClient.Create(apiKey, baseAddress, httpClient);

        // Assert
        Assert.IsNotNull(client);
    }

    [TestMethod]
    public void Create_WithBaseAddressWithoutPath_ReturnsProxyClient()
    {
        // Arrange
        var apiKey = "test-api-key";
        var baseAddress = "https://example.com";
        using var httpClient = new HttpClient();

        // Act
        var client = IProxyClient.Create(apiKey, baseAddress, httpClient);

        // Assert
        Assert.IsNotNull(client);
    }
}
