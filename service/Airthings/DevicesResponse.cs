namespace Tudormobile.Airthings;

public class DevicesResponse : ApiResponse
{
    public List<DeviceResponse> Devices { get; set; } = [];
}
