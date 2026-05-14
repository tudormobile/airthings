using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace Tudormobile.Airthings.Service;

internal class AirthingsApi
{
    private readonly string _apiKey;
    private readonly IAirthingsClient _client;
    private readonly ILogger _logger;
    private readonly IWebHostEnvironment _env;

    public AirthingsApi(string apiKey, IAirthingsClient client, ILogger logger, IWebHostEnvironment env)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiKey, nameof(apiKey));
        _apiKey = apiKey;
        _client = client;
        _logger = logger;
        _env = env;
    }

    internal Task<IResult> GetVersionAsync(HttpContext context, string apiKey)
        => HandleApiRequest(context, apiKey, nameof(GetVersionAsync), async () =>
        {
            return Results.Ok(AirthingsResponse.Success(new ServiceVersion()));
        });

    internal Task<IResult> GetDevicesAsync(HttpContext context, string apiKey)
        => HandleApiRequest(context, apiKey, nameof(GetDevicesAsync), async () =>
        {
            var (failure, data) = await FetchAccountDevicesAsync(context, nameof(GetDevicesAsync));
            if (failure is not null) return failure;

            var reply = data.SelectMany(x => x.Devices.Devices.Select(d => new Device
            {
                SerialNumber = d.SerialNumber,
                Home = d.Home,
                Name = d.Name
            })).ToList();

            return Results.Ok(AirthingsResponse.Success(reply));
        });

    internal Task<IResult> GetSamplesAsync(HttpContext context, string apiKey)
        => HandleApiRequest(context, apiKey, nameof(GetSamplesAsync), async () =>
        {
            var (failure, data) = await FetchAccountDevicesAsync(context, nameof(GetSamplesAsync));
            if (failure is not null) return failure;

            var reply = new List<DeviceSamples>();
            foreach (var (accountId, devices) in data)
            {
                var response = await _client.ReadSensors(accountId, [.. devices.Devices.Select(d => d.SerialNumber)]);
                if (response.IsSuccess)
                {
                    response.Results.ForEach(r => reply.Add(new DeviceSamples
                    {
                        SerialNumber = r.SerialNumber,
                        Recorded = r.Recorded,
                        BatteryPercentage = r.BatteryPercentage,
                        Samples = [.. r.Sensors.Select(s => new DeviceSample
                        {
                            SensorType = s.SensorType,
                            Value = s.Value,
                        })]
                    }));
                }
                else
                {
                    _logger.LogError("AirthingsService, {CallerName}, {RemoteIpAddress}, {AccountId}, Failed to read sensors, {ErrorMessage}",
                        nameof(GetSamplesAsync), context.Connection.RemoteIpAddress, accountId, response.Message);
                    return Results.Ok(AirthingsResponse.Failure(response.Message));
                }
            }
            return Results.Ok(AirthingsResponse.Success(reply));
        });

    // Fetches all accounts and their devices. Returns a failure IResult on any upstream error,
    // or null + a populated list of (accountId, devices) pairs on success.
    private async Task<(IResult? Failure, List<(string AccountId, DevicesResponse Devices)> Data)>
        FetchAccountDevicesAsync(HttpContext context, string callerName)
    {
        var empty = new List<(string, DevicesResponse)>();

        var accounts = await _client.ListAccounts();
        if (!accounts.IsSuccess)
        {
            _logger.LogError("AirthingsService, {CallerName}, {RemoteIpAddress}, Failed to list accounts, {ErrorMessage}",
                callerName, context.Connection.RemoteIpAddress, accounts.Message);
            return (Results.Ok(AirthingsResponse.Failure(accounts.Message)), empty);
        }

        var data = new List<(string AccountId, DevicesResponse Devices)>();
        foreach (var account in accounts.Accounts)
        {
            var devices = await _client.ListDevices(account.Id);
            if (!devices.IsSuccess)
            {
                _logger.LogError("AirthingsService, {CallerName}, {RemoteIpAddress}, {AccountId}, Failed to list devices, {ErrorMessage}",
                    callerName, context.Connection.RemoteIpAddress, account.Id, devices.Message);
                return (Results.Ok(AirthingsResponse.Failure(devices.Message)), empty);
            }
            data.Add((account.Id, devices));
        }

        return (null, data);
    }

    private async Task<IResult> HandleApiRequest(HttpContext context, string apiKey, string callerName, Func<Task<IResult>> onAuthorized)
    {
        LogApiRequest(context, callerName);
        var expectedBytes = System.Text.Encoding.UTF8.GetBytes(_apiKey);
        var providedBytes = System.Text.Encoding.UTF8.GetBytes(apiKey);
        if (CryptographicOperations.FixedTimeEquals(expectedBytes, providedBytes))
        {
            try
            {
                return await onAuthorized();
            }
            catch (Exception ex)
            {
                LogException(context, ex, callerName);
                return Results.Ok(AirthingsResponse.Failure(ex.Message));
            }
        }
        var redactedKey = apiKey.Length > 4 ? $"{apiKey[..4]}..." : "???";
        _logger.LogError("AirthingsService, {CallerName}, {RemoteIpAddress}, {ApiKey}, INVALID API KEY", callerName, context.Connection.RemoteIpAddress, redactedKey);
        return Results.Unauthorized();
    }

    private void LogApiRequest(HttpContext context, [CallerMemberName] string callerName = "")
    {
        _logger.LogInformation("AirthingsService, {CallerName}, {RemoteIpAddress}",
            callerName, context.Connection.RemoteIpAddress);
    }

    private void LogException(HttpContext context, Exception ex, [CallerMemberName] string callerName = "")
    {
        _logger.LogError(ex, "AirthingsService, {CallerName}, {RemoteIpAddress}",
            callerName, context.Connection.RemoteIpAddress);
    }

}
