using System.Diagnostics.CodeAnalysis;

namespace AirthingsService.Tests;

[ExcludeFromCodeCoverage]
internal class MockAirthingsClient : IAirthingsClient
{
    public Exception? AlwaysThrows { get; set; }
    public AccountsResponse Accounts { get; set; } = new AccountsResponse();
    public DevicesResponse Devices { get; set; } = new DevicesResponse();
    public DevicesSamplesResponse Samples { get; set; } = new DevicesSamplesResponse();
    public void Dispose() { }

    public Task<AccountsResponse> ListAccounts(CancellationToken cancellationToken = default)
        => AlwaysThrows != null ? throw AlwaysThrows : Task.FromResult(Accounts);

    public Task<DevicesResponse> ListDevices(string accountId, CancellationToken cancellationToken = default)
        => AlwaysThrows != null ? throw AlwaysThrows : Task.FromResult(Devices);

    public Task<DevicesSamplesResponse> ReadSensors(string accountId, IEnumerable<string> serialNumbers, UnitsType unitsType = UnitsType.Metric, CancellationToken cancellationToken = default)
        => AlwaysThrows != null ? throw AlwaysThrows : Task.FromResult(Samples);
}
