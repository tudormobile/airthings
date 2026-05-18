# Getting Started

### Installation
```bash
dotnet add package Tudormobile.Airthings

-or-

dotnet add package Tudormobile.Airthings.Service
```

### Prerequisites
- **.NET 10.0** or later
- Airthings **Client Id** and **Client Secret** - to access the Airthings consumer Api
- **HttpClient** - For making HTTP requests

### Quick Start

#### Basic API Usage:
```cs
using Tudormobile.Airthings;

// Create an instance of HttpClient to be used for making API requests.
using var httpClient = new HttpClient();

// Replace "your_client_id" and "your_client_secret" with your actual respective id and secret.
using var client = new AirthingsClient(httpClient, "your_client_id", "your_client_secret");
```
Tokens are sutomatically generarted or renewed as needed. You can retrieve the current token using the `AccessToken` property. These tokens are typically short lived and the client is designed to perform token management.


#### Airthings Proxy Service:
```cs
using Tudormobile.Airthings.Service

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAirthingsService(builder.Configuration);

var app = builder.Build();
app.UseAirthingsService();

app.Run();
```
The following endpoint are added:
- `/home/airthings/v1/status`
    - returns the status of the proxy service.
- `/home/airthings/v1/devices`
    - returns a collection of devices.
- `/home/airthings/v1/samples`
    - returns a collection of device samples.
