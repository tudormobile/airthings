using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Airthings.Service.Tests;

[TestClass]
public class AirthingsServiceExtensionsTests
{
    [TestMethod]
    public void AddAirthingsService_RegistersRequiredServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
            [
                new KeyValuePair<string, string?>("Airthings:ClientId", "test-client-id"),
                new KeyValuePair<string, string?>("Airthings:ClientSecret", "test-secret"),
                new KeyValuePair<string, string?>("Airthings:ApiKey", "test-api-key")
            ])
            .Build();

        // Act
        services.AddAirthingsService(configuration);
        var provider = services.BuildServiceProvider();

        // Assert
        var httpClientFactory = provider.GetService<IHttpClientFactory>();
        var options = provider.GetService<IOptions<AirthingsOptions>>();
        var airthingsClient = provider.GetService<AirthingsClient>();

        Assert.IsNotNull(httpClientFactory);
        Assert.IsNotNull(options);
        Assert.IsNotNull(airthingsClient);
        Assert.AreEqual("test-client-id", options.Value.ClientId);
        Assert.AreEqual("test-secret", options.Value.ClientSecret);
        Assert.AreEqual("test-api-key", options.Value.ApiKey);
    }

    [TestMethod]
    public void AddAirthingsService_FallsBackToEnvironmentVariables()
    {
        // Arrange
        var previousClientId = Environment.GetEnvironmentVariable("AIRTHINGS_CLIENT_ID");
        var previousClientSecret = Environment.GetEnvironmentVariable("AIRTHINGS_CLIENT_SECRET");

        try
        {
            Environment.SetEnvironmentVariable("AIRTHINGS_CLIENT_ID", "env-client-id");
            Environment.SetEnvironmentVariable("AIRTHINGS_CLIENT_SECRET", "env-secret");

            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder().Build();

            // Act
            services.AddAirthingsService(configuration);
            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<AirthingsOptions>>();

            // Assert
            Assert.AreEqual("env-client-id", options.Value.ClientId);
            Assert.AreEqual("env-secret", options.Value.ClientSecret);
        }
        finally
        {
            Environment.SetEnvironmentVariable("AIRTHINGS_CLIENT_ID", previousClientId);
            Environment.SetEnvironmentVariable("AIRTHINGS_CLIENT_SECRET", previousClientSecret);
        }
    }
}

