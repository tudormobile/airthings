using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using System.Net;
using Tudormobile.Airthings.Proxy;

namespace Airthings.IntegrationTests;

[TestClass]
[TestCategory("Integration")]

public class AirthingsServiceIntegrationTests : IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private HttpClient _client;

    public AirthingsServiceIntegrationTests()
    {
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((context, config) =>
                {
                    var clientId = Environment.GetEnvironmentVariable("AIRTHINGS_CLIENT_ID")
                        ?? throw new InvalidOperationException("AIRTHINGS_CLIENT_ID environment variable is required for integration tests");
                    var clientSecret = Environment.GetEnvironmentVariable("AIRTHINGS_CLIENT_SECRET")
                        ?? throw new InvalidOperationException("AIRTHINGS_CLIENT_SECRET environment variable is required for integration tests");

                    config.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["Airthings:ClientId"] = clientId,
                        ["Airthings:ClientSecret"] = clientSecret,
                        ["Airthings:ApiKey"] = "test-api-key"
                    });
                });
            });

        _client = _factory.CreateClient();
    }

    [TestInitialize]
    public void TestInitialize()
    {
        // Create a fresh client for each test to avoid header pollution
        _client?.Dispose();
        _client = _factory.CreateClient();
    }

    [TestMethod]
    public async Task StatusEndpoint_ReturnsOk_WithValidApiKey()
    {
        // Arrange
        _client.DefaultRequestHeaders.Add("ApiKey", "test-api-key");

        // Act
        var response = await _client.GetAsync("/home/airthings/v1/status", TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    public async Task StatusEndpoint_ReturnsUnauthorized_WithoutApiKey()
    {
        // Act
        var response = await _client.GetAsync("/home/airthings/v1/status", TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [TestMethod]
    public async Task DevicesEndpoint_IsMappedCorrectly()
    {
        // Arrange
        _client.DefaultRequestHeaders.Add("ApiKey", "test-api-key");

        // Act
        var response = await _client.GetAsync("/home/airthings/v1/devices", TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    public async Task SamplesEndpoint_ReturnsOk_WithValidApiKey()
    {
        // Arrange
        _client.DefaultRequestHeaders.Add("ApiKey", "test-api-key");

        // Act
        var response = await _client.GetAsync("/home/airthings/v1/samples", TestContext.CancellationToken);

        // Assert
        Assert.IsNotNull(response);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    public async Task StatusEndpointViaProxy_ReturnsOk_WithValidApiKey()
    {
        // Arrange
        var proxyClient = IProxyClient.Create("test-api-key", _client.BaseAddress!.ToString(), _client);


        // Act
        var response = await proxyClient.ReadStatus(TestContext.CancellationToken);

        // Assert
        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccess);
        Assert.IsNotEmpty(response.Version);
        Assert.IsNotNull(response.Samples);
        Assert.IsEmpty(response.Samples);
    }

    [TestMethod]
    public async Task SummaryEndpointViaProxy_ReturnsOk_WithValidApiKey()
    {
        // Arrange
        var proxyClient = IProxyClient.Create("test-api-key", _client.BaseAddress!.ToString(), _client);


        // Act
        var response = await proxyClient.ReadSummary(cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccess);
        Assert.IsNotEmpty(response.Version);
        Assert.IsNotNull(response.Samples);
        Assert.IsNotEmpty(response.Samples);
    }

    [TestMethod]
    public async Task SummaryEndpoint_ParsesUnitsParameter_WithValidUnits()
    {
        // Arrange
        _client.DefaultRequestHeaders.Add("ApiKey", "test-api-key");

        // Act
        var metricResponse = await _client.GetAsync("/home/airthings/v1/summary/metric", TestContext.CancellationToken);
        var imperialResponse = await _client.GetAsync("/home/airthings/v1/summary/imperial", TestContext.CancellationToken);

        // Assert
        Assert.IsNotNull(metricResponse);
        Assert.IsNotNull(imperialResponse);
        Assert.AreEqual(HttpStatusCode.OK, metricResponse.StatusCode);
        Assert.AreEqual(HttpStatusCode.OK, imperialResponse.StatusCode);
    }

    [TestMethod]
    public async Task SummaryEndpoint_DefaultsToImperial_WhenUnitsMissing()
    {
        // Arrange
        _client.DefaultRequestHeaders.Add("ApiKey", "test-api-key");

        // Act - Call summary without units parameter
        var response = await _client.GetAsync("/home/airthings/v1/summary", TestContext.CancellationToken);

        // Assert - Should default to Imperial and return OK
        Assert.IsNotNull(response);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    public async Task SummaryEndpoint_DefaultsToImperial_WhenUnitsInvalid()
    {
        // Arrange
        _client.DefaultRequestHeaders.Add("ApiKey", "test-api-key");

        // Act - Call summary with invalid/typo units parameter
        var response = await _client.GetAsync("/home/airthings/v1/summary/metricc", TestContext.CancellationToken); // typo

        // Assert - Should default to Imperial and return OK
        Assert.IsNotNull(response);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    public void Dispose()
    {
        _client?.Dispose();
        _factory?.Dispose();
    }

    public TestContext TestContext { get; set; }
}
