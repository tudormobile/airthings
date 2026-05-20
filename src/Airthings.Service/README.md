# Airthings Service
[![NuGet](https://img.shields.io/nuget/v/Tudormobile.Airthings.Service.svg)](https://www.nuget.org/packages/Tudormobile.Airthings.Service/)
[![License](https://img.shields.io/github/license/tudormobile/Airthings)](https://github.com/tudormobile/Airthings/blob/main/LICENSE.txt)
[![.NET](https://img.shields.io/badge/.NET-10.0-blue.svg)](https://dotnet.microsoft.com/download)

Web services library to access Airthings sensors.

## Installation
Install via NuGet Package Manager:

```sh
dotnet add package Tudormobile.Airthings.Service
```

Or via Package Manager Console:

```powershell
Install-Package Tudormobile.Airthings.Service
```

Or add directly to your `.csproj`:

```xml
<PackageReference Include="Tudormobile.AirthingsService" Version="1.0.0" />
```

## Quick Start
```cs
using Tudormobile.Airthings.Service;

var builder = WebApplication.CreateBuilder(args);
builder.AddAirthingsService(builder.Configuration);
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
`/summary` - Returns a summary of available sensor data  

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
#### /summary Endpoint
**Response**:  
Failure: The Data object is a string representing a general error message.  
Success: The Data object is an array of `SummarySamples`.  
```cs
public sealed record SummarySamples
{
    public DateTimeOffset LastUpdated { get; }
    public ServiceVersion Version { get; } 
    public List<SummarySample> Samples { get; }
}

public sealed record SummarySample
{
    public string Home { get; }
    public double Name { get; }
    public double Radon { get; }
    public double Humidity { get; }
    public double Temperature { get; }
}
```
```json
{
    "lastUpdated": "2026-05-19T21:17:50.9051184+00:00",
    "version": {
      "name": "AirthingsService",
      "description": "Web services API layer for Airthings applications",
      "copyright": "COPYRIGHT(C)2026 BILL TUDOR",
      "version": "1.0.0"
    },
    "samples": [
      {
        "home": "My Home",
        "name": "My Home Device 1",
        "radon": 4.57,
        "humidity": 42,
        "temperature": 65.2
      },
      {
        "home": "My Home",
        "name": "My Home Device 2",
        "radon": 1.38,
        "humidity": 49,
        "temperature": 74.6
      }
    ]
}
```

### Service (Proxy) Client
A dedicated service client is provided as a service access abstraction for client software written in ***dotnet***.

> [!NOTE]
> The ServiceClient is not yet implemented.

## Design Constraints and Known Limitations

> [!IMPORTANT]
> This service is intentionally designed for **single-user, single-account** deployment. The following constraints
> are by design and should be understood before deploying in any multi-user or multi-tenant scenario.

### Single-Account / Single-User
The service is configured with a fixed `ClientId` and `ClientSecret` at startup (via configuration or environment
variables). There is no mechanism to supply per-request credentials, so all API calls are made on behalf of the
same Airthings account. Multi-tenant use is not supported.

### Pagination Not Implemented
The Airthings API supports paginated results (`HasNext`, `TotalPages`). Pagination is **not** implemented because
the target deployment has a single account with a small, known number of devices that fits within a single page.
If the number of accounts or devices grows beyond a single page, results will be silently truncated. This must
be revisited before deploying at larger scale.

### Concurrent Token Refresh
The OAuth2 token refresh logic is serialized by a semaphore but does not implement a double-check guard. Under
concurrent load two requests could each trigger a token refresh in sequence. This is acceptable because output
caching limits upstream Airthings API calls to at most once every 30–60 minutes, making concurrent 401 responses
extremely unlikely in practice.

### Rate Limiting
No explicit client-side rate limiting is implemented. The output cache (30-minute TTL for samples, 60-minute
TTL for devices and status) acts as the primary guard against excessive upstream API calls.
