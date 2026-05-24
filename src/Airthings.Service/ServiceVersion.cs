using System.Reflection;

namespace Tudormobile.Airthings.Service;

/// <summary>
/// Represents version and metadata information for the Airthings Service.
/// </summary>
public record ServiceVersion
{
    private const string SERVICE_NAME = "AirthingsService";
    private const string SERVICE_DESCRIPTION = "Web services API layer for Airthings applications";
    private const string SERVICE_COPYRIGHT = "COPYRIGHT(C)2026 BILL TUDOR";
    private static readonly Lazy<string> _version = new(() =>
    {
        var v = Assembly.GetExecutingAssembly().GetName().Version;
        return v == null ? "0.0.0" : $"{v.Major}.{v.Minor}.{v.Build}";
    });

    /// <summary>
    /// Gets the name of the service.
    /// </summary>
    public string Name => SERVICE_NAME;

    /// <summary>
    /// Gets a description of the service functionality.
    /// </summary>
    public string Description => SERVICE_DESCRIPTION;

    /// <summary>
    /// Gets the copyright notice for the service.
    /// </summary>
    public string Copyright => SERVICE_COPYRIGHT;

    /// <summary>
    /// Gets the version number of the service assembly.
    /// </summary>
    public string Version => _version.Value;
}
