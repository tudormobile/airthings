using System.Net;

namespace Airthings.Tests;

[TestClass]
public class AirthingsClientTests
{
    [TestMethod]
    public void AirthingsClient_ConstructorTest()
    {
        // Arrange
        using var httpClient = new HttpClient();
        var clientSecret = "Client Secret";
        var clientId = "Client Id";
        var accessToken = "Access Token";
        var client = new AirthingsClient(httpClient, clientId, clientSecret, accessToken);

        Assert.AreEqual(accessToken, client.AccessToken);
        Assert.IsInstanceOfType<IAirthingsClient>(client);
    }

    [TestMethod]
    public async Task AirthingsClient_ListAccounts_WithBadAuth_ReturnsUnauthorized()
    {
        // Arrange
        var handler = new MockHttpMessageHandler() { AlwaysResponds = new HttpResponseMessage(HttpStatusCode.Unauthorized) };
        using var httpClient = new HttpClient(handler);
        var clientSecret = "Client Secret";
        var clientId = "Client Id";
        var accessToken = "Access Token";
        var client = new AirthingsClient(httpClient, clientId, clientSecret, accessToken);

        // Act
        var response = await client.ListAccounts(TestContext.CancellationToken);
        Assert.IsFalse(response.IsSuccess);
        Assert.StartsWith("Network error:", response.Message);
    }

    [TestMethod]
    public async Task AirthingsClient_ListAccounts_ReturnsSuccess()
    {
        // Arrange
        var handler = new MockHttpMessageHandler()
        {
            AlwaysResponds = new HttpResponseMessage(HttpStatusCode.OK)
        };
        using var httpClient = new HttpClient(handler);
        var clientSecret = "Client Secret";
        var clientId = "Client Id";
        var accessToken = "Access Token";
        var client = new AirthingsClient(httpClient, clientId, clientSecret, accessToken);

        // Act
        var response = await client.ListAccounts(TestContext.CancellationToken);

        // Assert
        Assert.IsTrue(response.IsSuccess);
    }

    [TestMethod]
    public async Task AirthingsClient_ListAccounts_WithError_ReturnsFailure()
    {
        // Arrange
        var handler = new MockHttpMessageHandler()
        {
            AlwaysResponds = new HttpResponseMessage(HttpStatusCode.InternalServerError)
        };
        using var httpClient = new HttpClient(handler);
        var clientSecret = "Client Secret";
        var clientId = "Client Id";
        var accessToken = "Access Token";
        var client = new AirthingsClient(httpClient, clientId, clientSecret, accessToken);

        // Act
        var response = await client.ListAccounts(TestContext.CancellationToken);

        // Assert
        Assert.IsFalse(response.IsSuccess);
    }

    [TestMethod]
    public async Task AirthingsClient_ListAccounts_WithAuth_ReturnsAccountsAndUpdatesToken()
    {
        // Arrange
        var updatedToken = Guid.NewGuid().ToString();
        var handler = new MockHttpMessageHandler()
        {
            AlwaysResponds = new HttpResponseMessage(HttpStatusCode.OK),
            AuthUpdatedToken = updatedToken,
        };
        using var httpClient = new HttpClient(handler);
        var clientSecret = "Client Secret";
        var clientId = "Client Id";
        var accessToken = "Access Token";
        var client = new AirthingsClient(httpClient, clientId, clientSecret, accessToken);

        // Act
        var response = await client.ListAccounts(TestContext.CancellationToken);

        // Assert
        Assert.IsTrue(response.IsSuccess);
        Assert.AreEqual(updatedToken, client.AccessToken);
        Assert.AreEqual(clientSecret, handler.ProvidedClientSecret);
        Assert.AreEqual(clientId, handler.ProvidedClientId);
    }

    [TestMethod]
    public async Task AirthingsClient_ListAccounts_WithErrorResponse_ReturnsFailure()
    {
        // Arrange
        var handler = new MockHttpMessageHandler()
        {
            JsonResponse = @"
{
    ""message"": ""error message""
}"
        };
        using var httpClient = new HttpClient(handler);
        var clientSecret = "Client Secret";
        var clientId = "Client Id";
        var accessToken = "Access Token";
        var client = new AirthingsClient(httpClient, clientId, clientSecret, accessToken);

        // Act
        var response = await client.ListAccounts(TestContext.CancellationToken);

        // Assert
        Assert.IsFalse(response.IsSuccess);
        Assert.AreEqual("error message", response.Message);
        Assert.AreEqual("https://consumer-api.airthings.com/v1/accounts", handler.ProvidedRequestUri!.AbsoluteUri);
    }

    [TestMethod]
    public async Task AirthingClient_ListAccounts_ReturnsAccounts()
    {
        // Arrange
        var uri = "https://consumer-api.airthings.com/v1/accounts";
        var json = @"
{
  ""accounts"": [
    {
      ""id"": ""12345""
    }
  ]
}";
        var handler = new MockHttpMessageHandler() { JsonResponse = json };
        using var httpClient = new HttpClient(handler);
        var clientSecret = "Client Secret";
        var clientId = "Client Id";
        var accessToken = "Access Token";
        var client = new AirthingsClient(httpClient, clientId, clientSecret, accessToken);

        // Act
        var response = await client.ListAccounts(TestContext.CancellationToken);

        // Assert
        Assert.IsTrue(response.IsSuccess);
        Assert.HasCount(1, response.Accounts);
        Assert.AreEqual("12345", response.Accounts.First().Id);
        Assert.AreEqual(uri, handler.ProvidedRequestUri!.AbsoluteUri);
    }

    [TestMethod]
    public async Task AirthingClient_ListDevices_ReturnsDevices()
    {
        // Arrange
        var accountId = "12345";
        var uri = $"https://consumer-api.airthings.com/v1/accounts/{accountId}/devices";
        var json = @"
{
  ""devices"": [
    {
      ""serialNumber"": ""abc"",
      ""home"": ""some home"",
      ""name"": ""some name"",
      ""type"": ""some type"",
      ""sensors"": [
        ""67890""
      ]
    }
  ]
}";
        var handler = new MockHttpMessageHandler() { JsonResponse = json };
        using var httpClient = new HttpClient(handler);
        var clientSecret = "Client Secret";
        var clientId = "Client Id";
        var accessToken = "Access Token";
        var client = new AirthingsClient(httpClient, clientId, clientSecret, accessToken);

        // Act
        var response = await client.ListDevices(accountId, TestContext.CancellationToken);

        // Assert
        Assert.IsTrue(response.IsSuccess);
        Assert.HasCount(1, response.Devices);
        Assert.AreEqual("abc", response.Devices.First().SerialNumber);
        Assert.AreEqual("some home", response.Devices[0].Home);
        Assert.AreEqual("some name", response.Devices[0].Name);
        Assert.AreEqual("some type", response.Devices[0].Type);
        Assert.HasCount(1, response.Devices[0].Sensors);
        Assert.AreEqual("67890", response.Devices[0].Sensors[0]);
        Assert.AreEqual(uri, handler.ProvidedRequestUri!.AbsoluteUri);
    }

    [TestMethod]
    public async Task AirthingClient_ReadSensors_ReturnsSensors()
    {
        // Arrange
        var accountId = "12345";
        var device1 = "0123456789";
        var device2 = "2989037410";
        var uri = $"https://consumer-api.airthings.com/v1/accounts/{accountId}/sensors?device={device1}&device={device2}";
        var json = @"
{
    ""results"": [
        {
            ""serialNumber"": ""0123456789"",
            ""sensors"": [
                { ""sensorType"": ""radonShortTermAvg"", ""value"": 123.456,""unit"": ""bq""   },
                { ""sensorType"": ""humidity"",          ""value"": 47.5,   ""unit"": ""pct""  },
                { ""sensorType"": ""temp"",              ""value"": 16.7,   ""unit"": ""c""    },
                { ""sensorType"": ""co2"",               ""value"": 416.0,  ""unit"": ""ppm""  },
                { ""sensorType"": ""voc"",               ""value"": 144.0,  ""unit"": ""ppb""  },
                { ""sensorType"": ""pressure"",          ""value"": 1009.6, ""unit"": ""mbar"" },
                { ""sensorType"": ""pm25"",              ""value"": 1.0,    ""unit"": ""mgpc"" },
                { ""sensorType"": ""pm1"",               ""value"": 1.0,    ""unit"": ""mgpc"" }
            ],
            ""recorded"": ""2026-05-12T17:56:56"",
            ""batteryPercentage"": 84
        },
        {
            ""serialNumber"": ""2989037410"",
            ""sensors"": [
                { ""sensorType"": ""radonShortTermAvg"", ""value"": 118.25,""unit"": ""bq""  },
                { ""sensorType"": ""humidity"",          ""value"": 45.6,  ""unit"": ""pct"" },
                { ""sensorType"": ""temp"",              ""value"": 20.4,  ""unit"": ""c""   }
            ],
            ""recorded"": ""2026-05-12T18:02:00"",
            ""batteryPercentage"": 96
        }
    ],
    ""hasNext"": false,
    ""totalPages"": 1
}
";
        var handler = new MockHttpMessageHandler() { JsonResponse = json };
        using var httpClient = new HttpClient(handler);
        var clientSecret = "Client Secret";
        var clientId = "Client Id";
        var accessToken = "Access Token";
        var client = new AirthingsClient(httpClient, clientId, clientSecret, accessToken);

        // Act
        var response = await client.ReadSensors(accountId, ["0123456789", "2989037410"], cancellationToken: TestContext.CancellationToken);

        // Assert

        // Basic API URI validation and response parsing
        Assert.IsTrue(response.IsSuccess);
        Assert.AreEqual(uri, handler.ProvidedRequestUri!.AbsoluteUri);
        Assert.HasCount(2, response.Results);
        Assert.IsFalse(response.HasNext);
        Assert.AreEqual(1, response.TotalPages);

        // Validate first device's first sensor and metadata to ensure correct parsing
        Assert.AreEqual("0123456789", response.Results[0].SerialNumber);
        Assert.AreEqual("radonShortTermAvg", response.Results[0].Sensors[0].SensorType);
        Assert.AreEqual(123.456, response.Results[0].Sensors[0].Value, double.Epsilon);
        Assert.AreEqual("bq", response.Results[0].Sensors[0].Unit);
        Assert.AreEqual(84, response.Results[0].BatteryPercentage);
        Assert.AreEqual("humidity", response.Results[0].Sensors[1].SensorType);
        Assert.AreEqual(47.5, response.Results[0].Sensors[1].Value, double.Epsilon);
        Assert.AreEqual("pct", response.Results[0].Sensors[1].Unit);
        Assert.AreEqual("temp", response.Results[0].Sensors[2].SensorType);
        Assert.AreEqual(16.7, response.Results[0].Sensors[2].Value, double.Epsilon);
        Assert.AreEqual("c", response.Results[0].Sensors[2].Unit);
        Assert.AreEqual(DateTime.Parse("2026-05-12T17:56:56"), response.Results[0].Recorded);

        // Validate second device's second sensor and metadata to ensure correct parsing
        Assert.AreEqual("2989037410", response.Results[1].SerialNumber);
        Assert.AreEqual("radonShortTermAvg", response.Results[1].Sensors[0].SensorType);
        Assert.AreEqual(118.25, response.Results[1].Sensors[0].Value, double.Epsilon);
        Assert.AreEqual("humidity", response.Results[1].Sensors[1].SensorType);
        Assert.AreEqual(45.6, response.Results[1].Sensors[1].Value, double.Epsilon);
        Assert.AreEqual("pct", response.Results[1].Sensors[1].Unit);
        Assert.AreEqual("temp", response.Results[1].Sensors[2].SensorType);
        Assert.AreEqual(20.4, response.Results[1].Sensors[2].Value, double.Epsilon);
        Assert.AreEqual("c", response.Results[1].Sensors[2].Unit);
        Assert.AreEqual(96, response.Results[1].BatteryPercentage);
        Assert.AreEqual(DateTime.Parse("2026-05-12T18:02:00"), response.Results[1].Recorded);

    }

    [TestMethod]
    public async Task AirthingClient_ReadSensors_WithMalformedJson_ReturnsError()
    {
        // Arrange
        var accountId = "12345";
        var uri = $"https://consumer-api.airthings.com/v1/accounts/{accountId}/sensors?device=0123456789";
        var json = @"
{
    ""serialNumber"":""0123456789"",
this is malformed json
    ""sensors"":[
        {
            ""sensorType"":""radonShortTermAvg"",
            ""value"":123.456,
            ""unit"":""bq""
        }
    ],
    ""recorded"":""2026-05-12T17:56:56"",
    ""batteryPercentage"":84
}";
        var handler = new MockHttpMessageHandler() { JsonResponse = json };
        using var httpClient = new HttpClient(handler);
        var clientSecret = "Client Secret";
        var clientId = "Client Id";
        var accessToken = "Access Token";
        var client = new AirthingsClient(httpClient, clientId, clientSecret, accessToken);

        // Act
        var response = await client.ReadSensors(accountId, ["0123456789"], cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.IsFalse(response.IsSuccess);
        Assert.StartsWith("Invalid JSON: ", response.Message);
        Assert.HasCount(0, response.Results);
        Assert.AreEqual(uri, handler.ProvidedRequestUri!.AbsoluteUri);
    }

    [TestMethod]
    public async Task AirthingClient_ReadSensors_WithJsonNull_ReturnsError()
    {
        // Arrange
        var accountId = "12345";
        var uri = $"https://consumer-api.airthings.com/v1/accounts/{accountId}/sensors?device=0123456789";
        var json = @"null";
        var handler = new MockHttpMessageHandler() { JsonResponse = json };
        using var httpClient = new HttpClient(handler);
        var clientSecret = "Client Secret";
        var clientId = "Client Id";
        var accessToken = "Access Token";
        var client = new AirthingsClient(httpClient, clientId, clientSecret, accessToken);

        // Act
        var response = await client.ReadSensors(accountId, ["0123456789"], cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.IsFalse(response.IsSuccess);
        Assert.HasCount(0, response.Results);
        Assert.AreEqual("Request failed.", response.Message);
        Assert.AreEqual(uri, handler.ProvidedRequestUri!.AbsoluteUri);
    }

    [TestMethod]
    public async Task AirthingsClient_ListAccounts_WithMissingTokenInResponse_ReturnsNetworkError()
    {
        // Arrange - token endpoint returns JSON without an access_token property
        var handler = new MockHttpMessageHandler()
        {
            AuthUpdatedToken = "trigger-401",
            TokenJsonResponse = @"{ ""token_type"": ""Bearer"" }"
        };
        using var httpClient = new HttpClient(handler);
        var client = new AirthingsClient(httpClient, "clientId", "clientSecret");

        // Act
        var response = await client.ListAccounts(TestContext.CancellationToken);

        // Assert
        Assert.IsFalse(response.IsSuccess);
        Assert.StartsWith("Network error: ", response.Message);
    }

    [TestMethod]
    public async Task AirthingsClient_ListAccounts_WithMalformedTokenResponse_ReturnsNetworkError()
    {
        // Arrange - token endpoint returns malformed JSON
        var handler = new MockHttpMessageHandler()
        {
            AuthUpdatedToken = "trigger-401",
            TokenJsonResponse = @"this is not valid json {"
        };
        using var httpClient = new HttpClient(handler);
        var client = new AirthingsClient(httpClient, "clientId", "clientSecret");

        // Act
        var response = await client.ListAccounts(TestContext.CancellationToken);

        // Assert
        Assert.IsFalse(response.IsSuccess);
        Assert.StartsWith("Network error: Token refresh failed: Malformed response.", response.Message);
    }

    public TestContext TestContext { get; set; }    // set by MSTest
}
