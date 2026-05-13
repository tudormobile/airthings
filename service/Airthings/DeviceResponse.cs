namespace Tudormobile.Airthings;

public class DeviceResponse
{
    public string SerialNumber { get; set; } = string.Empty;
    public string Home { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public List<string> Sensors { get; set; } = [];
}
