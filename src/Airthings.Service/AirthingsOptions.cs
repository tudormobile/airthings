namespace Tudormobile.Airthings.Service;

/// <summary>
/// Configuration options for the Airthings Service.
/// </summary>
public class AirthingsOptions
{
    /// <summary>Gets or sets the API key used to authenticate callers of this service.</summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>Gets or sets the Airthings OAuth2 client ID.</summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>Gets or sets the Airthings OAuth2 client secret.</summary>
    public string ClientSecret { get; set; } = string.Empty;
}
