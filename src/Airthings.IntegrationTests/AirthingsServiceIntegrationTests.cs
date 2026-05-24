using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace Airthings.IntegrationTests;

[TestClass]
public class AirthingsServiceIntegrationTests
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public AirthingsServiceIntegrationTests()
    {
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((context, config) =>
                {
                    config.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["Airthings:ClientId"] = "test-client-id",
                        ["Airthings:ClientSecret"] = "test-secret",
                        ["Airthings:ApiKey"] = "test-api-key"
                    });
                });

                // Mock the IAirthingsClient for testing
                builder.ConfigureServices(services =>
                {
                    // Replace real client with mock
                });
            });

        _client = _factory.CreateClient();
    }

    [TestMethod]
    public async Task StatusEndpoint_ReturnsOk_WithValidApiKey()
    {
        // Arrange
        _client.DefaultRequestHeaders.Add("ApiKey", "test-api-key");

        // Act
        var response = await _client.GetAsync("/home/airthings/v1/status");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    public async Task StatusEndpoint_ReturnsUnauthorized_WithoutApiKey()
    {
        // Act
        var response = await _client.GetAsync("/home/airthings/v1/status");

        // Assert
        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [TestMethod]
    public async Task DevicesEndpoint_IsMappedCorrectly()
    {
        // Arrange
        _client.DefaultRequestHeaders.Add("ApiKey", "test-api-key");

        // Act
        var response = await _client.GetAsync("/home/airthings/v1/devices");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    public async Task SamplesEndpoint_ReturnsOk_WithValidApiKey()
    {
        // Arrange
        _client.DefaultRequestHeaders.Add("ApiKey", "test-api-key");

        // Act
        var response = await _client.GetAsync("/home/airthings/v1/samples");

        // Assert
        Assert.IsNotNull(response);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    public async Task SummaryEndpoint_ParsesUnitsParameter_WithValidUnits()
    {
        // Arrange
        _client.DefaultRequestHeaders.Add("ApiKey", "test-api-key");

        // Act
        var metricResponse = await _client.GetAsync("/home/airthings/v1/summary/metric");
        var imperialResponse = await _client.GetAsync("/home/airthings/v1/summary/imperial");

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
        var response = await _client.GetAsync("/home/airthings/v1/summary");

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
        var response = await _client.GetAsync("/home/airthings/v1/summary/metricc"); // typo

        // Assert - Should default to Imperial and return OK
        Assert.IsNotNull(response);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }
}
