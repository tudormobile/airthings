namespace Airthings.Tests.Proxy;

[TestClass]
public class ProxyClientTests
{
    [TestMethod]
    public void ProxyClient_ConstructWithInvalidBaseAddress_Throws()
    {
        // Arrange
        using var httpClient = new HttpClient();
        var apiKey = "sample-api-key";
        var baseAddress = "invalid..example.com";

        // Act
        var ex = Assert.ThrowsExactly<ArgumentException>(() => IProxyClient.Create(apiKey, baseAddress, httpClient));

        // Assert
        Assert.AreEqual("baseAddress", ex.ParamName);
    }

    [TestMethod]
    public async Task ProxyClient_ReadStatus_ReturnsProxyResponse()
    {
        // Arrange
        var handler = new MockHttpMessageHandler()
        {
            JsonResponse = @"{
  ""isSuccess"": true,
  ""data"": {
    ""name"": ""AirthingsService"",
    ""description"": ""Web services API layer for Airthings applications"",
    ""copyright"": ""COPYRIGHT(C)2026 BILL TUDOR"",
    ""version"": ""1.2.3""
  }
}"
        };

        using var httpClient = new HttpClient(handler);
        var apiKey = "sample-api-key";
        var baseAddress = "https://api.example.com";
        var client = IProxyClient.Create(apiKey, baseAddress, httpClient);

        // Act
        var response = await client.ReadStatus(TestContext.CancellationToken);

        // Assert
        Assert.IsTrue(response.IsSuccess);
        Assert.IsEmpty(response.Samples);
        Assert.AreEqual("1.2.3", response.Version);
        Assert.StartsWith(baseAddress, handler.ProvidedRequestUri!.ToString());
        Assert.EndsWith("/status", handler.ProvidedRequestUri.ToString());
    }

    [TestMethod]
    public async Task ProxyClient_ReadSummary_WithNetworkError_ReturnsProxyResponse()
    {
        // Arrange
        var handler = new MockHttpMessageHandler()
        {
            JsonResponse = @"{
  ""isSuccess"": false,
  ""data"": ""Network error: No such host is known. (consumer-api.airthings.com:443)""
}"
        };

        using var httpClient = new HttpClient(handler);
        var apiKey = "sample-api-key";
        var baseAddress = "https://api.example.com";
        var unitsType = UnitsType.Imperial;
        var client = IProxyClient.Create(apiKey, baseAddress, httpClient);

        // Act
        var response = await client.ReadSummary(unitsType, TestContext.CancellationToken);

        // Assert
        Assert.IsFalse(response.IsSuccess);
        Assert.AreEqual("Network error: No such host is known. (consumer-api.airthings.com:443)", response.Message);
        Assert.StartsWith(baseAddress, handler.ProvidedRequestUri!.ToString());
        Assert.EndsWith("/summary/imperial", handler.ProvidedRequestUri.ToString());
    }

    [TestMethod]
    public async Task ProxyClient_ReadSummary_ReturnsProxyResponse()
    {
        // Arrange
        var handler = new MockHttpMessageHandler()
        {
            JsonResponse = @"{
  ""isSuccess"": true,
  ""data"": {
    ""lastUpdated"": ""2026-05-25T12:08:49.0205192+00:00"",
    ""version"": {
      ""name"": ""AirthingsService"",
      ""description"": ""Web services API layer for Airthings applications"",
      ""copyright"": ""COPYRIGHT(C)2026 BILL TUDOR"",
      ""version"": ""4.5.6""
    },
    ""samples"": [
      {
        ""home"": ""Some Home"",
        ""name"": ""Basement"",
        ""radon"": 5.05,
        ""humidity"": 42,
        ""temperature"": 60,
        ""recorded"": ""2026-05-25T11:57:06"",
        ""batteryPercentage"": 81
      },
      {
        ""home"": ""Some Home"",
        ""name"": ""Kitchen"",
        ""radon"": 5.27,
        ""humidity"": 45,
        ""temperature"": 60.1,
        ""recorded"": ""2026-05-25T12:02:13"",
        ""batteryPercentage"": 95
      }
    ]
  }
}"
        };

        using var httpClient = new HttpClient(handler);
        var apiKey = "sample-api-key";
        var baseAddress = "https://api.example.com";
        var unitsType = UnitsType.Imperial;
        var client = IProxyClient.Create(apiKey, baseAddress, httpClient);

        // Act
        var response = await client.ReadSummary(unitsType, TestContext.CancellationToken);

        // Assert
        Assert.IsTrue(response.IsSuccess);
        Assert.IsNull(response.Message);
        Assert.AreEqual("4.5.6", response.Version);
        Assert.StartsWith(baseAddress, handler.ProvidedRequestUri!.ToString());
        Assert.EndsWith("/summary/imperial", handler.ProvidedRequestUri.ToString());
        Assert.HasCount(2, response.Samples);

        // First Sample
        Assert.AreEqual("Some Home", response.Samples[0].Home);
        Assert.AreEqual("Basement", response.Samples[0].Name);
        Assert.AreEqual(5.05, response.Samples[0].Radon);
        Assert.AreEqual(42, response.Samples[0].Humidity);
        Assert.AreEqual(60, response.Samples[0].Temperature);
        Assert.AreEqual(new DateTime(2026, 5, 25, 11, 57, 6), response.Samples[0].Recorded);
        Assert.AreEqual(81, response.Samples[0].BatteryPercentage);

        // Second Sample
        Assert.AreEqual("Some Home", response.Samples[1].Home);
        Assert.AreEqual("Kitchen", response.Samples[1].Name);
        Assert.AreEqual(5.27, response.Samples[1].Radon);
        Assert.AreEqual(45, response.Samples[1].Humidity);
        Assert.AreEqual(60.1, response.Samples[1].Temperature);
        Assert.AreEqual(new DateTime(2026, 5, 25, 12, 2, 13), response.Samples[1].Recorded);
        Assert.AreEqual(95, response.Samples[1].BatteryPercentage);
    }

    public TestContext TestContext { get; set; }    // Set by mstest
}
