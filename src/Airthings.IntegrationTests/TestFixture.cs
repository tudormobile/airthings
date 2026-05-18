using System.Diagnostics.CodeAnalysis;

namespace Airthings.IntegrationTests;

/// <summary>
/// Provides shared test infrastructure for all integration tests.
/// </summary>
[TestClass, ExcludeFromCodeCoverage]
public class TestFixture
{
    private static HttpClient? _sharedHttpClient;
    private static AirthingsClient? _sharedClient;

    /// <summary>
    /// Gets the shared HttpClient instance for all integration tests.
    /// </summary>
    public static HttpClient SharedHttpClient => _sharedHttpClient
        ?? throw new InvalidOperationException("Test fixture not initialized");

    /// <summary>
    /// Gets the shared AirthingsClient instance for all integration tests.
    /// </summary>
    public static AirthingsClient SharedClient => _sharedClient
        ?? throw new InvalidOperationException("Test fixture not initialized");

    /// <summary>
    /// Initializes shared resources once for the entire test assembly.
    /// </summary>
    [AssemblyInitialize]
    public static void AssemblyInitialize(TestContext context)
    {
        // Create a single HttpClient for all integration tests
        _sharedHttpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(30) // Reasonable timeout for API calls
        };

        // Create a shared AirthingsClient
        var clientId = Environment.GetEnvironmentVariable("AIRTHINGS_CLIENT_ID") ?? "";
        var clientSecret = Environment.GetEnvironmentVariable("AIRTHINGS_CLIENT_SECRET") ?? "";
        _sharedClient = new AirthingsClient(_sharedHttpClient, clientId, clientSecret);

        context.WriteLine("Integration test assembly initialized");
        context.WriteLine($"Using real Airthings API for client ID: {clientId}");
    }

    /// <summary>
    /// Cleans up shared resources after all tests complete.
    /// </summary>
    [AssemblyCleanup]
    public static void AssemblyCleanup()
    {
        _sharedHttpClient?.Dispose();
        _sharedClient?.Dispose();
        _sharedHttpClient = null;
        _sharedClient = null;
    }
    [TestMethod]
    public void CreateClient_ShouldSucceed()
    {
        // Arrange & Act
        var client = new AirthingsClient(SharedHttpClient, "dummyClientId", "dummyClientSecret");
        // Assert
        Assert.IsNotNull(client);
    }
}