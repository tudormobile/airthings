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
    public async Task AirthingsClient_GetHealth_WithBadAuth_Throws()
    {
        // Arrange
        var handler = new MockHttpMessageHandler() { AlwaysResponds = new HttpResponseMessage(HttpStatusCode.Unauthorized) };
        using var httpClient = new HttpClient(handler);
        var clientSecret = "Client Secret";
        var clientId = "Client Id";
        var accessToken = "Access Token";
        var client = new AirthingsClient(httpClient, clientId, clientSecret, accessToken);

        // Act
        var exception = await Assert.ThrowsAsync<HttpRequestException>(() => client.GetHealth(TestContext.CancellationToken));
        Assert.AreEqual(HttpStatusCode.Unauthorized, exception.StatusCode);
    }

    [TestMethod]
    public async Task AirthingClient_GetHealth_ReturnsSuccess()
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
        var response = await client.GetHealth(TestContext.CancellationToken);

        // Assert
        Assert.IsTrue(response.IsSuccess);
    }

    [TestMethod]
    public async Task AirthingClient_GetHealth_WithError_ReturnsFailureHealth()
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
        var response = await client.GetHealth(TestContext.CancellationToken);

        // Assert
        Assert.IsFalse(response.IsSuccess);
    }

    [TestMethod]
    public async Task AirthingClient_GetHealth_WithAuth_ReturnsHealthAndUpdatesToken()
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
        var response = await client.GetHealth(TestContext.CancellationToken);

        // Assert
        Assert.IsTrue(response.IsSuccess);
        Assert.AreEqual(updatedToken, client.AccessToken);
        Assert.AreEqual(clientSecret, handler.ProvidedClientSecret);
        Assert.AreEqual(clientId, handler.ProvidedClientId);
    }

    [TestMethod]
    public async Task AirthingClient_GetHealth_WithErrorResponse_ReturnsFailureHealth()
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
        var response = await client.GetHealth(TestContext.CancellationToken);

        // Assert
        Assert.IsFalse(response.IsSuccess);
        Assert.AreEqual("error message", response.Message);
        Assert.AreEqual("https://consumer-api.airthings.com/v1/health", handler.ProvidedRequestUri!.AbsoluteUri);
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
        var uri = $"https://consumer-api.airthings.com/v1/accounts/{accountId}/sensors?device=0123456789";
        var json = @"
{
    ""serialNumber"":""0123456789"",
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
        var response = await client.ReadSensors(accountId, ["0123456789"], TestContext.CancellationToken);

        // Assert
        Assert.IsTrue(response.IsSuccess);
        Assert.HasCount(1, response.Sensors);
        Assert.AreEqual("0123456789", response.SerialNumber);
        Assert.AreEqual("radonShortTermAvg", response.Sensors[0].SensorType);
        Assert.AreEqual(123.456, response.Sensors[0].Value);
        Assert.AreEqual("bq", response.Sensors[0].Unit);
        Assert.AreEqual(84, response.BatteryPercentage);
        Assert.AreEqual(DateTime.Parse("2026-05-12T17:56:56"), response.Recorded);
        Assert.AreEqual(uri, handler.ProvidedRequestUri!.AbsoluteUri);
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
        var response = await client.ReadSensors(accountId, ["0123456789"], TestContext.CancellationToken);

        // Assert
        Assert.IsFalse(response.IsSuccess);
        Assert.StartsWith("Invalid JSON: ", response.Message);
        Assert.HasCount(0, response.Sensors);
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
        var response = await client.ReadSensors(accountId, ["0123456789"], TestContext.CancellationToken);

        // Assert
        Assert.IsFalse(response.IsSuccess);
        Assert.HasCount(0, response.Sensors);
        Assert.AreEqual("Request failed.", response.Message);
        Assert.AreEqual(uri, handler.ProvidedRequestUri!.AbsoluteUri);
    }

    public TestContext TestContext { get; set; }    // set by MSTest
}
