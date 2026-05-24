using System.Diagnostics.CodeAnalysis;

namespace AirthingsService.Tests;

[ExcludeFromCodeCoverage]
internal class MockAirthingsClient : IAirthingsClient
{
    public Exception? AlwaysThrows { get; set; }
    public Exception? DevicesAlwaysThrows { get; set; }
    public AccountsResponse Accounts { get; set; } = new AccountsResponse();
    public DevicesResponse Devices { get; set; } = new DevicesResponse();
    public DevicesSamplesResponse Samples { get; set; } = new DevicesSamplesResponse();

    // Allow setting a specific DevicesResponse per account (useful for testing failure scenarios)
    public Dictionary<string, DevicesResponse> DevicesByAccount { get; set; } = new Dictionary<string, DevicesResponse>();

    // Allow setting a specific DevicesSamplesResponse per account (useful for testing failure scenarios)
    public Dictionary<string, DevicesSamplesResponse> SamplesByAccount { get; set; } = new Dictionary<string, DevicesSamplesResponse>();

    public void Dispose() { }

    public Task<AccountsResponse> ListAccounts(CancellationToken cancellationToken = default)
        => AlwaysThrows != null ? throw AlwaysThrows : Task.FromResult(Accounts);

    public Task<DevicesResponse> ListDevices(string accountId, CancellationToken cancellationToken = default)
    {
        if (AlwaysThrows != null) throw AlwaysThrows;
        if (DevicesAlwaysThrows != null) throw DevicesAlwaysThrows;

        // Check if there's a specific response for this account
        if (DevicesByAccount.TryGetValue(accountId, out var accountDevices))
            return Task.FromResult(accountDevices);

        return Task.FromResult(Devices);
    }

    public Task<DevicesSamplesResponse> ReadSensors(string accountId, IEnumerable<string> serialNumbers, UnitsType unitsType = UnitsType.Metric, CancellationToken cancellationToken = default)
    {
        if (AlwaysThrows != null) throw AlwaysThrows;

        // Check if there's a specific samples response for this account
        if (SamplesByAccount.TryGetValue(accountId, out var accountSamples))
            return Task.FromResult(accountSamples);

        return Task.FromResult(Samples);
    }
}
