namespace Tudormobile.Airthings;

/// <summary>
/// Represents an Airthings account.
/// </summary>
/// <remarks>
/// Mapping of the Airthings API response.
/// </remarks>
public record Account
{
    /// <summary>
    /// Gets or sets the unique identifier of the account.
    /// </summary>
    public string Id { get; set; } = string.Empty;
}
