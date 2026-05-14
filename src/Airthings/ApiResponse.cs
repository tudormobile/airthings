namespace Tudormobile.Airthings;

/// <summary>
/// Base class for all Airthings API responses.
/// </summary>
/// <remarks>
/// Mapping of the Airthings API response.
/// </remarks>
public record ApiResponse
{
    /// <summary>
    /// Gets or sets an error message returned by the API, or <see langword="null"/> if the request succeeded.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Gets a value indicating whether the request was successful.
    /// </summary>
    /// <remarks>
    /// Returns <see langword="true"/> when <see cref="Message"/> is <see langword="null"/>.
    /// None of the current Airthings API endpoints return a message on success, so this property
    /// is implemented to catch the error replies, which all return a simple message. Exceptions
    /// will aslo use the message property to return the error details, so this is a catch-all for any failed request. 
    /// <para>
    /// If the API adds successful messages in the future, this logic will need to be updated along with the 
    /// API response object builders to check for specific errors instead, and explicitly set the success status. 
    /// For now, this is sufficient to catch all errors, and the API does not return any messages on success.
    /// </para>
    /// </remarks>
    public bool IsSuccess => Message == null;
}
