namespace SimpleConsolseApp;

using Tudormobile.Airthings;
using Tudormobile.Airthings.Proxy;

internal class Program
{
    static async Task Main(string[] args)
    {
        // Get the API key and base address from environment variables, or from hardcoded values (not recommended for production)
        var apiKey = Environment.GetEnvironmentVariable("AIRTHINGS_API_KEY") ?? "replace_with_your_api_key";
        var basAddress = Environment.GetEnvironmentVariable("AIRTHINGS_BASE_ADDRESS") ?? "https://api.example.com";
        using var httpClient = new HttpClient() { Timeout = TimeSpan.FromSeconds(30) };

        // Additional configuration for the HttpClient can be done here, such as setting default headers, if needed
        httpClient.DefaultRequestHeaders.Add("x-tudormobile-api", Environment.GetEnvironmentVariable("TUDORMOBILE_API_KEY") ?? "replace_with_your_tudormobile_api_key");

        // Create the Airthings API proxy client
        var client = IProxyClient.Create(apiKey, basAddress, httpClient);

        // Read & display the summary data from the proxy
        var summary = await client.ReadSummary(UnitsType.Imperial);

        Console.WriteLine($"Airthings Proxy Version - {(string.IsNullOrEmpty(summary.Version) ? "OFFLINE" : summary.Version)}");
        Console.WriteLine($"Airthings Success: {summary.IsSuccess}");
        Console.WriteLine($"Airthings Message: {summary.Message ?? "N/A"}");
        Console.WriteLine($"Data Last Updated: {summary.LastUpdated}");
        Console.WriteLine($"{summary.Samples.Count} device sample(s) found:");

        foreach (var sample in summary.Samples)
        {
            Console.WriteLine($"- Device: {sample.Home} ({sample.Name})");
            Console.WriteLine($"  - Radon: {sample.Radon} pCi/L");
            Console.WriteLine($"  - Temperature: {sample.Temperature} °F");
            Console.WriteLine($"  - Humidity: {sample.Humidity} %");
        }
    }
}
