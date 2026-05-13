namespace Tudormobile.Airthings;

public class ApiResponse
{
    public string? Message { get; set; }
    public bool IsSuccess => Message == null;
    internal static ApiResponse HealthyResponse => new();
    internal static ApiResponse ErrorResponse(string? message = null) => new() { Message = message ?? "Request failed." };
}
