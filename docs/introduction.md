# Introduction

```
using Tudormobile.Airthings;
```

## Overview

**Tudormobile.Airthings** is a C# SDK for accessing the Airthings consumer api. The Api is consumed by obtaining an instance of the `IAirthingsClient` interface.  

See also: [Generated Documentation](api/Tudormobile.md) for the `Tudormobile.Airthings` library.

### IAirthingsClient Interface
```cs
public interface IAirthingsClient : IDisposable
{
    public Task<AccountsResponse> ListAccounts();
    public Task<DevicesResponse> ListDevices(string accountId);
    public Task<DevicesSamplesResponse> ReadSensors(
        string accountId, 
        IEnumerable<string> serialNumbers, 
        UnitsType unitsType);
}
```
### Response objects
All methods are asynchronous and return a task representing a response object derived from `ApiResponse`.
```cs
public record ApiResponse
{
    public bool IsSuccess { get; }
    public string? Message { get; }
}
```
Always check the `IsSuccess` property to ensure valid data has been returned. In the event of an error, `IsSuccess` will return `false` and the error message will be returned in the `Message` property. In the case of success, the `Message` property will be `null`.

- `AccountsResponse` - A list of `Account` objects
- `DevicesResponse` - A list of `Device` objects
- `DevicesSamplesReponse` - A list of `Sensor` objects

### Authorization
The Airthings consumer api requires application registration. See the [Airthings documentation](https://consumer-api-doc.airthings.com/docs/api/getting-started) for details. Provide your provided *clientId* and *clientSecret* when creating the `AirthingsClient` object. Authorization tokens are silently managed for you when accessing the api. You can obtain the token for persistence via the `AccessToken` property, however, these tokens are typically short lived. The `AirthingsClient` is designed as a long-lived disposable object that can be shared throughout your application lifetime.

### Airthings Service

```
using Tudormobile.Airthings.Service;
```

It is not expected that front-end applications will be built using the `IAirthingsClient` client since it requires both the *client-id* and *client-secret* values. 

The ***Airtings.Service.dll*** allows dotnet web applications to act as a proxy for front-end applications to access the Airthings consumer api. Client credentials remain on the server, and a simplified api is exposed. This api also aggressively cache's data to avoid Airthings rate limits.

### Service endpoints
/home/airthings/v1 -> airthings service

`/status` - Returns web service status information  
`/devices` - Returns collection of available devices  
`/samples` - Returns all available sensor data  
`/summary` - Returns a summary of available sensor data  

See also: [Generated documentation](api/Tudormobile.Airthings.Service.yml) for the `Tudormobile.Airthings.Service` library.