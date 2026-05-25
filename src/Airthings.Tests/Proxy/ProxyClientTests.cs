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

    [TestMethod]
    public async Task ProxyClient_ReadStatus_WithFailureResponse_ReturnsError()
    {
        // Arrange
        var handler = new MockHttpMessageHandler()
        {
            JsonResponse = @"{
  ""isSuccess"": false,
  ""data"": ""Service temporarily unavailable""
}"
        };

        using var httpClient = new HttpClient(handler);
        var apiKey = "sample-api-key";
        var baseAddress = "https://api.example.com";
        var client = IProxyClient.Create(apiKey, baseAddress, httpClient);

        // Act
        var response = await client.ReadStatus(TestContext.CancellationToken);

        // Assert
        Assert.IsFalse(response.IsSuccess);
        Assert.AreEqual("Service temporarily unavailable", response.Message);
        Assert.StartsWith(baseAddress, handler.ProvidedRequestUri!.ToString());
        Assert.EndsWith("/status", handler.ProvidedRequestUri.ToString());
    }

    [TestMethod]
    public async Task ProxyClient_ReadSummary_WithMetricUnits_ReturnsProxyResponse()
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
      ""version"": ""3.2.1""
    },
    ""samples"": [
      {
        ""home"": ""Test Home"",
        ""name"": ""Living Room"",
        ""radon"": 150.5,
        ""humidity"": 50,
        ""temperature"": 21.5,
        ""recorded"": ""2026-05-25T14:30:00"",
        ""batteryPercentage"": 88
      }
    ]
  }
}"
        };

        using var httpClient = new HttpClient(handler);
        var apiKey = "sample-api-key";
        var baseAddress = "https://api.example.com";
        var unitsType = UnitsType.Metric;
        var client = IProxyClient.Create(apiKey, baseAddress, httpClient);

        // Act
        var response = await client.ReadSummary(unitsType, TestContext.CancellationToken);

        // Assert
        Assert.IsTrue(response.IsSuccess);
        Assert.IsNull(response.Message);
        Assert.AreEqual("3.2.1", response.Version);
        Assert.StartsWith(baseAddress, handler.ProvidedRequestUri!.ToString());
        Assert.EndsWith("/summary/metric", handler.ProvidedRequestUri.ToString());
        Assert.HasCount(1, response.Samples);
        Assert.AreEqual("Living Room", response.Samples[0].Name);
        Assert.AreEqual(150.5, response.Samples[0].Radon);
    }

    [TestMethod]
    public async Task ProxyClient_ReadStatus_WithMalformedJson_ReturnsError()
    {
        // Arrange
        var handler = new MockHttpMessageHandler()
        {
            JsonResponse = @"{ ""isSuccess"": true, ""data"": { invalid json"
        };

        using var httpClient = new HttpClient(handler);
        var apiKey = "sample-api-key";
        var baseAddress = "https://api.example.com";
        var client = IProxyClient.Create(apiKey, baseAddress, httpClient);

        // Act
        var response = await client.ReadStatus(TestContext.CancellationToken);

        // Assert
        Assert.IsFalse(response.IsSuccess);
        Assert.IsNotNull(response.Message);
        Assert.StartsWith("Invalid JSON:", response.Message);
    }

    [TestMethod]
    public async Task ProxyClient_ReadSummary_WithMalformedJson_ReturnsError()
    {
        // Arrange
        var handler = new MockHttpMessageHandler()
        {
            JsonResponse = @"{ not valid json }"
        };

        using var httpClient = new HttpClient(handler);
        var apiKey = "sample-api-key";
        var baseAddress = "https://api.example.com";
        var client = IProxyClient.Create(apiKey, baseAddress, httpClient);

        // Act
        var response = await client.ReadSummary(UnitsType.Metric, TestContext.CancellationToken);

        // Assert
        Assert.IsFalse(response.IsSuccess);
        Assert.IsNotNull(response.Message);
        Assert.StartsWith("Invalid JSON:", response.Message);
    }

    [TestMethod]
    public async Task ProxyClient_ReadSummary_WithEmptySamples_ReturnsProxyResponse()
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
      ""version"": ""2.0.0""
    },
    ""samples"": []
  }
}"
        };

        using var httpClient = new HttpClient(handler);
        var apiKey = "sample-api-key";
        var baseAddress = "https://api.example.com";
        var client = IProxyClient.Create(apiKey, baseAddress, httpClient);

        // Act
        var response = await client.ReadSummary(UnitsType.Imperial, TestContext.CancellationToken);

        // Assert
        Assert.IsTrue(response.IsSuccess);
        Assert.IsNull(response.Message);
        Assert.AreEqual("2.0.0", response.Version);
        Assert.IsEmpty(response.Samples);
    }

    [TestMethod]
    public async Task ProxyClient_ReadStatus_IncludesApiKeyHeader()
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
    ""version"": ""1.0.0""
  }
}"
        };

        using var httpClient = new HttpClient(handler);
        var apiKey = "test-api-key-12345";
        var baseAddress = "https://api.example.com";
        var client = IProxyClient.Create(apiKey, baseAddress, httpClient);

        // Act
        var response = await client.ReadStatus(TestContext.CancellationToken);

        // Assert
        Assert.IsTrue(response.IsSuccess);
        Assert.IsNotNull(handler.ProvidedRequestUri);
    }

    [TestMethod]
    public async Task ProxyClient_ReadSummary_IncludesApiKeyHeader()
    {
        // Arrange
        var handler = new MockHttpMessageHandler()
        {
            JsonResponse = @"{
  ""isSuccess"": true,
  ""data"": {
    ""version"": {
      ""version"": ""1.0.0""
    },
    ""samples"": []
  }
}"
        };

        using var httpClient = new HttpClient(handler);
        var apiKey = "test-api-key-67890";
        var baseAddress = "https://api.example.com";
        var client = IProxyClient.Create(apiKey, baseAddress, httpClient);

        // Act
        var response = await client.ReadSummary(UnitsType.Metric, TestContext.CancellationToken);

        // Assert
        Assert.IsTrue(response.IsSuccess);
        Assert.IsNotNull(handler.ProvidedRequestUri);
    }

    [TestMethod]
    public async Task ProxyClient_ReadStatus_WithCancellationToken_CompletesSuccessfully()
    {
        // Arrange
        var handler = new MockHttpMessageHandler()
        {
            JsonResponse = @"{
  ""isSuccess"": true,
  ""data"": {
    ""version"": ""1.0.0""
  }
}"
        };

        using var httpClient = new HttpClient(handler);
        var apiKey = "sample-api-key";
        var baseAddress = "https://api.example.com";
        var client = IProxyClient.Create(apiKey, baseAddress, httpClient);
        using var cts = new CancellationTokenSource();

        // Act
        var response = await client.ReadStatus(cts.Token);

        // Assert
        Assert.IsTrue(response.IsSuccess);
    }

    [TestMethod]
    public async Task ProxyClient_ReadSummary_WithCancellationToken_CompletesSuccessfully()
    {
        // Arrange
        var handler = new MockHttpMessageHandler()
        {
            JsonResponse = @"{
  ""isSuccess"": true,
  ""data"": {
    ""version"": { ""version"": ""1.0.0"" },
    ""samples"": []
  }
}"
        };

        using var httpClient = new HttpClient(handler);
        var apiKey = "sample-api-key";
        var baseAddress = "https://api.example.com";
        var client = IProxyClient.Create(apiKey, baseAddress, httpClient);
        using var cts = new CancellationTokenSource();

        // Act
        var response = await client.ReadSummary(UnitsType.Metric, cts.Token);

        // Assert
        Assert.IsTrue(response.IsSuccess);
    }

    [TestMethod]
    public async Task ProxyClient_ReadStatus_WithHttpError_ReturnsError()
    {
        // Arrange
        var handler = new MockHttpMessageHandler()
        {
            AlwaysResponds = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError)
            {
                Content = new StringContent("Internal Server Error")
            }
        };

        using var httpClient = new HttpClient(handler);
        var apiKey = "sample-api-key";
        var baseAddress = "https://api.example.com";
        var client = IProxyClient.Create(apiKey, baseAddress, httpClient);

        // Act
        var response = await client.ReadStatus(TestContext.CancellationToken);

        // Assert
        Assert.IsFalse(response.IsSuccess);
        Assert.IsNotNull(response.Message);
        Assert.StartsWith("Network error:", response.Message);
    }

    [TestMethod]
    public async Task ProxyClient_ReadSummary_WithHttpError_ReturnsError()
    {
        // Arrange
        var handler = new MockHttpMessageHandler()
        {
            AlwaysResponds = new HttpResponseMessage(System.Net.HttpStatusCode.NotFound)
            {
                Content = new StringContent("Not Found")
            }
        };

        using var httpClient = new HttpClient(handler);
        var apiKey = "sample-api-key";
        var baseAddress = "https://api.example.com";
        var client = IProxyClient.Create(apiKey, baseAddress, httpClient);

        // Act
        var response = await client.ReadSummary(UnitsType.Imperial, TestContext.CancellationToken);

        // Assert
        Assert.IsFalse(response.IsSuccess);
        Assert.IsNotNull(response.Message);
        Assert.StartsWith("Network error:", response.Message);
    }

    [TestMethod]
    public async Task ProxyClient_ReadStatus_WithNetworkException_ReturnsError()
    {
        // Arrange
        var handler = new MockHttpMessageHandler()
        {
            AlwaysThrows = new HttpRequestException("DNS resolution failed")
        };

        using var httpClient = new HttpClient(handler);
        var apiKey = "sample-api-key";
        var baseAddress = "https://api.example.com";
        var client = IProxyClient.Create(apiKey, baseAddress, httpClient);

        // Act
        var response = await client.ReadStatus(TestContext.CancellationToken);

        // Assert
        Assert.IsFalse(response.IsSuccess);
        Assert.IsNotNull(response.Message);
        Assert.AreEqual("Network error: DNS resolution failed", response.Message);
    }

    [TestMethod]
    public async Task ProxyClient_ReadSummary_WithNetworkException_ReturnsError()
    {
        // Arrange
        var handler = new MockHttpMessageHandler()
        {
            AlwaysThrows = new HttpRequestException("Connection timeout")
        };

        using var httpClient = new HttpClient(handler);
        var apiKey = "sample-api-key";
        var baseAddress = "https://api.example.com";
        var client = IProxyClient.Create(apiKey, baseAddress, httpClient);

        // Act
        var response = await client.ReadSummary(UnitsType.Metric, TestContext.CancellationToken);

        // Assert
        Assert.IsFalse(response.IsSuccess);
        Assert.IsNotNull(response.Message);
        Assert.AreEqual("Network error: Connection timeout", response.Message);
    }

    public TestContext TestContext { get; set; }    // Set by mstest
}
