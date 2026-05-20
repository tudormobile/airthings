using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace AirthingsService.Tests;

[TestClass]
public class AirthingsApiTests
{
    private const string ApiKey = "some_api_key";
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private ServiceProvider _services = null!;


    [TestInitialize]
    public void TestInitialize()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton(Options.Create(new JsonOptions()));
        serviceCollection.AddLogging();
        _services = serviceCollection.BuildServiceProvider();
    }

    [TestCleanup]
    public void TestCleanup() => _services.Dispose();

    [TestMethod]
    public async Task GetVersionAsync_ValidApiKey_ReturnsSuccess()
    {
        var context = CreateHttpContext();
        var client = new MockAirthingsClient();
        var env = new MockWebHostEnvironment();
        var api = new AirthingsApi(ApiKey, client, NullLogger<AirthingsApi>.Instance, env);

        var result = await api.GetVersionAsync(context, ApiKey);
        await result.ExecuteAsync(context);

        Assert.AreEqual(200, context.Response.StatusCode);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await JsonSerializer.DeserializeAsync<AirthingsResponse<ServiceVersion>>(
            context.Response.Body, JsonOptions, TestContext.CancellationToken);

        Assert.IsNotNull(body);
        Assert.IsTrue(body.IsSuccess);
        Assert.IsNotNull(body.Data);
    }

    [TestMethod]
    public async Task GetVersionAsync_InvalidApiKey_ReturnsUnauthorized()
    {
        var context = CreateHttpContext();
        var client = new MockAirthingsClient();
        var env = new MockWebHostEnvironment();
        var api = new AirthingsApi(ApiKey, client, NullLogger<AirthingsApi>.Instance, env);

        var result = await api.GetVersionAsync(context, "wrong_key");
        await result.ExecuteAsync(context);

        Assert.AreEqual(401, context.Response.StatusCode);
    }

    [TestMethod]
    public async Task GetDevicesAsync_ReturnsSuccess()
    {
        // Arrange
        var context = CreateHttpContext();
        var client = new MockAirthingsClient()
        {
            Accounts = new AccountsResponse() { Accounts = [new AccountResponse() { Id = "a1" }] },
            Devices = new DevicesResponse()
            {
                Devices = [new DeviceResponse() { Home = "home", Name = "name", SerialNumber = "12345", Type = "some type", Sensors = ["one", "two"] }]
            }
        };
        var env = new MockWebHostEnvironment();
        var options = _services.GetRequiredService<IOptions<JsonOptions>>();
        var api = new AirthingsApi(ApiKey, client, NullLogger<AirthingsApi>.Instance, env);

        // Act
        var result = await api.GetDevicesAsync(context, ApiKey);
        await result.ExecuteAsync(context);

        // Assert
        Assert.AreEqual(200, context.Response.StatusCode);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body);
        var json = await reader.ReadToEndAsync();
        var body = JsonSerializer.Deserialize<AirthingsResponse<List<Device>>>(json, JsonOptions);

        Assert.IsNotNull(body);
        Assert.IsTrue(body.IsSuccess);
        Assert.IsNotNull(body.Data);
        Assert.HasCount(1, body.Data);
        Assert.AreEqual("12345", body.Data[0].SerialNumber);
        Assert.AreEqual("home", body.Data[0].Home);
        Assert.AreEqual("name", body.Data[0].Name);
    }

    [TestMethod]
    public async Task GetSamplesAsync_ReturnsSuccess()
    {
        // Arrange
        var context = CreateHttpContext();
        var client = new MockAirthingsClient()
        {
            Accounts = new AccountsResponse() { Accounts = [new AccountResponse() { Id = "a1" }] },
            Devices = new DevicesResponse()
            {
                Devices = [new DeviceResponse() { Home = "home", Name = "name", SerialNumber = "12345", Type = "some type", Sensors = ["one", "two"] }]
            },
            Samples = new DevicesSamplesResponse()
            {
                HasNext = false,
                TotalPages = 1,
                Results = [
                    new SensorsResponse()
                    {
                        BatteryPercentage = 12,
                        Recorded = DateTime.Now,
                        SerialNumber = "12345",
                        Sensors = [
                            new SensorResponse() { SensorType = "one", Unit = "bq", Value = 65 },
                            new SensorResponse() { SensorType = "two", Unit = "c", Value = 42.42 }
                        ]
                    }
                ]
            }
        };
        var env = new MockWebHostEnvironment();
        var options = _services.GetRequiredService<IOptions<JsonOptions>>();
        var api = new AirthingsApi(ApiKey, client, NullLogger<AirthingsApi>.Instance, env);

        // Act
        var result = await api.GetSamplesAsync(context, ApiKey);
        await result.ExecuteAsync(context);

        // Assert
        Assert.AreEqual(200, context.Response.StatusCode);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body);
        var json = await reader.ReadToEndAsync();
        var body = JsonSerializer.Deserialize<AirthingsResponse<List<DeviceSamples>>>(json, JsonOptions);

        Assert.IsNotNull(body);
        Assert.IsTrue(body.IsSuccess);
        Assert.IsNotNull(body.Data);
        Assert.HasCount(1, body.Data);
        Assert.AreEqual("12345", body.Data[0].SerialNumber);
        Assert.HasCount(2, body.Data[0].Samples);

        Assert.AreEqual("one", body.Data[0].Samples[0].SensorType);
        Assert.AreEqual(65.0, body.Data[0].Samples[0].Value, double.Epsilon);
        Assert.AreEqual("two", body.Data[0].Samples[1].SensorType);
        Assert.AreEqual(42.42, body.Data[0].Samples[1].Value, double.Epsilon);
    }

    [TestMethod]
    public async Task GetSummaryAsync_ReturnsSuccess()
    {
        // Arrange
        var context = CreateHttpContext();
        var client = new MockAirthingsClient()
        {
            Accounts = new AccountsResponse() { Accounts = [new AccountResponse() { Id = "a1" }] },
            Devices = new DevicesResponse()
            {
                Devices = [new DeviceResponse() { Home = "home", Name = "name", SerialNumber = "12345", Type = "some type", Sensors = ["one", "two"] }]
            },
            Samples = new DevicesSamplesResponse()
            {
                HasNext = false,
                TotalPages = 1,
                Results = [
                    new SensorsResponse()
                    {
                        BatteryPercentage = 12,
                        Recorded = DateTime.Now,
                        SerialNumber = "12345",
                        Sensors = [
                            new SensorResponse() { SensorType = "radonShortTermAvg", Unit = "bq", Value = 65 },
                            new SensorResponse() { SensorType = "humidity", Unit = "%", Value = 42.0 },
                            new SensorResponse() { SensorType = "temp", Unit = "c", Value = 21.5 }
                        ]
                    }
                ]
            }
        };
        var env = new MockWebHostEnvironment();
        var api = new AirthingsApi(ApiKey, client, NullLogger<AirthingsApi>.Instance, env);

        // Act
        var result = await api.GetSummaryAsync(context, ApiKey, unitsType: UnitsType.Metric);
        await result.ExecuteAsync(context);

        // Assert
        Assert.AreEqual(200, context.Response.StatusCode);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body);
        var json = await reader.ReadToEndAsync();
        var body = JsonSerializer.Deserialize<AirthingsResponse<SummarySamples>>(json, JsonOptions);

        Assert.IsNotNull(body);
        Assert.IsTrue(body.IsSuccess);
        Assert.IsNotNull(body.Data);
        Assert.HasCount(1, body.Data.Samples);

        Assert.AreEqual("home", body.Data.Samples[0].Home);
        Assert.AreEqual("name", body.Data.Samples[0].Name);
        Assert.AreEqual(65.0, body.Data.Samples[0].Radon, double.Epsilon);
        Assert.AreEqual(42.0, body.Data.Samples[0].Humidity, double.Epsilon);
        Assert.AreEqual(21.5, body.Data.Samples[0].Temperature, double.Epsilon);
    }

    [TestMethod]
    public async Task GetDevicesAsync_ClientThrows_ReturnsError()
    {
        // Arrange
        var context = CreateHttpContext();
        var client = new MockAirthingsClient() { AlwaysThrows = new Exception("Test exception"), };
        var env = new MockWebHostEnvironment();
        var options = _services.GetRequiredService<IOptions<JsonOptions>>();
        var api = new AirthingsApi(ApiKey, client, NullLogger<AirthingsApi>.Instance, env);

        // Act
        var result = await api.GetDevicesAsync(context, ApiKey);
        await result.ExecuteAsync(context);

        // Assert
        Assert.AreEqual(200, context.Response.StatusCode);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body);
        var json = await reader.ReadToEndAsync();
        var body = JsonSerializer.Deserialize<AirthingsResponse<string>>(json, JsonOptions);

        Assert.IsNotNull(body);
        Assert.IsFalse(body.IsSuccess);
        Assert.AreEqual("Test exception", body.Data);
    }


    private DefaultHttpContext CreateHttpContext()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        context.RequestServices = _services;
        return context;
    }

    public TestContext TestContext { get; set; }
}
