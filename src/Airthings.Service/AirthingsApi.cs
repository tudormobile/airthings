using System.Runtime.CompilerServices;

namespace Tudormobile.Airthings.Service;

internal class AirthingsApi
{
    private readonly string _apiKey;
    private readonly AirthingsClient _client;
    private readonly ILogger _logger;
    private readonly IWebHostEnvironment _env;

    public AirthingsApi(string apiKey, AirthingsClient client, ILogger logger, IWebHostEnvironment env)
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
            var accounts = await _client.ListAccounts();
            if (accounts.IsSuccess)
            {
                var reply = new List<Device>();
                foreach (var account in accounts.Accounts)
                {
                    var devices = await _client.ListDevices(account.Id);
                    if (devices.IsSuccess)
                    {
                        var d = devices.Devices.Select(d => new Device
                        {
                            SerialNumber = d.SerialNumber,
                            Home = d.Home,
                            Name = d.Name
                        });
                        reply.AddRange(d);
                    }
                    else
                    {
                        _logger.LogError("AirthingsService, {CallerName}, {RemoteIpAddress}, {AccountId}, Failed to list devices, {ErrorMessage}",
                            nameof(GetDevicesAsync), context.Connection.RemoteIpAddress, account.Id, devices.Message);
                        return Results.Ok(AirthingsResponse.Failure(devices.Message));
                    }
                }
                return Results.Ok(AirthingsResponse.Success(reply));
            }
            _logger.LogError("AirthingsService, {CallerName}, {RemoteIpAddress}, Failed to list accounts, {ErrorMessage}",
                nameof(GetDevicesAsync), context.Connection.RemoteIpAddress, accounts.Message);
            return Results.Ok(AirthingsResponse.Failure(accounts.Message));
        });

    internal Task<IResult> GetSamplesAsync(HttpContext context, string apiKey)
        => HandleApiRequest(context, apiKey, nameof(GetSamplesAsync), async () =>
        {
            var accounts = await _client.ListAccounts();
            if (accounts.IsSuccess)
            {
                var reply = new List<DeviceSamples>();
                foreach (var account in accounts.Accounts)
                {
                    var devices = await _client.ListDevices(account.Id);
                    if (devices.IsSuccess)
                    {
                        var response = await _client.ReadSensors(account.Id, [.. devices.Devices.Select(d => d.SerialNumber)]);
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
                                nameof(GetSamplesAsync), context.Connection.RemoteIpAddress, account.Id, response.Message);
                            return Results.Ok(AirthingsResponse.Failure(response.Message));
                        }
                    }
                    else
                    {
                        _logger.LogError("AirthingsService, {CallerName}, {RemoteIpAddress}, {AccountId}, Failed to list devices, {ErrorMessage}",
                            nameof(GetSamplesAsync), context.Connection.RemoteIpAddress, account.Id, devices.Message);
                        return Results.Ok(AirthingsResponse.Failure(devices.Message));
                    }
                }
                return Results.Ok(AirthingsResponse.Success(reply));
            }
            _logger.LogError("AirthingsService, {CallerName}, {RemoteIpAddress}, Failed to list accounts, {ErrorMessage}",
                nameof(GetSamplesAsync), context.Connection.RemoteIpAddress, accounts.Message);
            return Results.Ok(AirthingsResponse.Failure(accounts.Message));
        });

    private async Task<IResult> HandleApiRequest(HttpContext context, string apiKey, string callerName, Func<Task<IResult>> onAuthorized)
    {
        LogApiRequest(context, callerName);
        if (apiKey == _apiKey)
        {
            return await onAuthorized();
        }
        _logger.LogError("AirthingsService, {CallerName}, {RemoteIpAddress}, {ApiKey}, INVALID API KEY", callerName, context.Connection.RemoteIpAddress, apiKey);
        return Results.NotFound();
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
