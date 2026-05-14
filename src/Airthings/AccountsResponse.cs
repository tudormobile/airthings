namespace Tudormobile.Airthings;

/// <summary>
/// Represents the response from a list accounts request.
/// </summary>
/// <remarks>
/// Mapping of the Airthings API response.
/// </remarks>
public record AccountsResponse : ApiResponse
{
    /// <summary>
    /// Gets or sets the collection of accounts accessible with the current credentials.
    /// </summary>
    public List<AccountResponse> Accounts { get; set; } = [];
}
