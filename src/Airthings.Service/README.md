# Airthings Service
Web services library to access Airthings sensors.

## Quick Start
```cs
using Tudormobile.Airthings.Service;

var builder = WebApplication.CreateBuilder(args);
// ...
var app = builder.Build();
app.UseAirthingsService();
// ...
app.Run();
```
### Web Service Endpoints

/home/airthings/v1 -> airthings service

`/status` - Returns web service status information  
`/devices` - Returns collection of available devices  
`/samples` - Returns all available sensor data

> [!NOTE]
> All endpoints will cache data for at least 15 minutes to avoid 
> rate limits accessing the Airthings web services.

Use the devices endpoint to build a map of devices identified by their serial numbers. Use this information when reading the sensors to associated them with their respective device. All Json is formatted as ***snake_case*** (lower).

### Response Objects
All service requests use a common format for the response object:
```json
{
    "is_success" : bool,
    "data" : object | null
}
```
The data object varies depending on success and the specific endpoint.
```cs
public sealed record AirthingsResponse<T>
{
    public bool IsSuccess { get; init; }
    public T? Data { get; init; }
}
```
#### /status Endpoint
**Response**:
Always returns success. The Data object is an instance of `ServiceVersion`.
```cs
public record ServiceVersion
{
    public string Name { get; }
    public string Description { get; }
    public string Copyright { get; }
    public string Version { get; }
}
```
```json
{
    "name" : string,
    "description" : string,
    "copyright" : string,
    "version" : string
}
```

#### /devices Endpoint
**Response**:  
Failure: The Data object is a string representing a general error message.  
Success: The Data object is an array of `Device`.  
```cs
public record Device
{
    public string SerialNumber { get; }
    public string Home { get; }
    public string Name { get; }
}
```
```json
{
    "serial_number" : string,
    "name" : string,
    "home" : string
}
```
#### /samples Endpoint
**Response**:  
Failure: The Data object is a string representing a general error message.  
Success: The Data object is an array of `DeviceSamples`.  
```cs
public record DeviceSamples
{
    public string SerialNumber { get; }
    public List<DeviceSensor> Samples { get; }
    public DateTime Recorded { get; }
    public int BatteryPercentage { get; }
}

public sealed record DeviceSample
{
    public string SensorType { get; }
    public double Value { get; }
}
```
```json
{
    "serial_number" : string,
    "samples" : [array of DeviceSample objecs]
    "recorded" : string (UTC date string),
    "battery_percentage" : number (double)
}

{
    "sensor_type" : string,
    "value" : number (double)
}
```
### Service Client
A dedicated service client is provided as a service access abstraction for client software written in ***dotnet***.

> [!NOTE]
> The ServiceClient is not yet implemented.
