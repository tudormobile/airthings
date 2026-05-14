namespace Tudormobile.Airthings;

/// <summary>
/// Represents the response from a list devices request.
/// </summary>
/// <remarks>
/// Mapping of the Airthings API response.
/// </remarks>
public record DevicesResponse : ApiResponse
{
    /// <summary>
    /// Gets or sets the collection of devices associated with the requested account.
    /// </summary>
    public List<DeviceResponse> Devices { get; set; } = [];
}
