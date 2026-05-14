namespace Tudormobile.Airthings;

/// <summary>
/// Provides methods for interacting with the Airthings API to manage accounts, devices, and sensor data.
/// </summary>
public interface IAirthingsClient : IDisposable
{
    /// <summary>
    /// Lists all accounts accessible with the current credentials.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the accounts response with a list of accounts.</returns>
    public Task<AccountsResponse> ListAccounts(CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all devices associated with the specified account.
    /// </summary>
    /// <param name="accountId">The unique identifier of the account.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the devices response with a list of devices.</returns>
    public Task<DevicesResponse> ListDevices(string accountId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reads sensor data from the specified devices.
    /// </summary>
    /// <param name="accountId">The unique identifier of the account.</param>
    /// <param name="serialNumbers">The serial numbers of the devices to read sensor data from.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the sensors response with sensor data.</returns>
    public Task<DevicesSamplesResponse> ReadSensors(string accountId, IEnumerable<string> serialNumbers, CancellationToken cancellationToken = default);
}
