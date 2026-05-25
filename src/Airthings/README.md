# Airthings
[![NuGet](https://img.shields.io/nuget/v/Tudormobile.Airthings.svg)](https://www.nuget.org/packages/Tudormobile.Airthings/)
[![License](https://img.shields.io/github/license/tudormobile/Airthings)](https://github.com/tudormobile/Airthings/blob/main/LICENSE.txt)
[![.NET](https://img.shields.io/badge/.NET-10.0-blue.svg)](https://dotnet.microsoft.com/download)

Client access and object model for the Airthings Consumer API.

## Installation
Install via NuGet Package Manager:

```sh
dotnet add package Tudormobile.Airthings
```

Or via Package Manager Console:

```powershell
Install-Package Tudormobile.Airthings
```

Or add directly to your `.csproj`:

```xml
<PackageReference Include="Tudormobile.Airthings" Version="1.0.0" />
```

## Quick Start
### Airthings Client
An *airthings client* is provided for direct access the the Airthings Consumer API. The client requires the Client Identifier and Client Secret of your Airthings registered application. Methods on the client are provided for all endpoints in the consumer api.
```cs
using Tudormobile.Airthings;

var clientId = "YOUR_CLIENT_ID";
var clientSecret = "YOUR_CLIENT_SECRET";
using var httpClient = new HttpClient();

using var client = new AirthingsClient(httpClient, clientId, clientSecret);

var accounts = await client.ListAccounts();
// ...
```

### Proxy Client
A *proxy client* is provided for accessing the Airthings Consumer API through the proxy service provided by the `Tudormobile.Airthings.Service` package. Client credentials are contained within the service implementation, which is accessed through an *api key* for the proxy. A simplified api with aggregated sensor data response is provided by the proxy for a single Client Identifier and Client Secret.
```cs
using Tudormobile.Airthings.Proxy;

var apiKey = "replace-with-api-key";
using var client = new HttoClient();

var proxy = new IProxyClient.Create(apiKey, httpClient);

var response = await proxy.ReadSummary();
if (response.IsSuccess)
{
    foreach (var sample in response.Samples)
    {
        // ...
    }
}
```

## Documentation
- **[Complete Documentation](https://tudormobile.github.io/airthings/)** - Comprehensive guides and API reference
- **[API Documentation](https://tudormobile.github.io/airthings/api/Tudormobile.html)** - Generated API reference

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/tudormobile/Airthings/blob/main/LICENSE.txt) file for details.

