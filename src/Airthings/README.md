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
```cs
using Tudormobile.Airthings;

var clientId = "YOUR_CLIENT_ID";
var clientSecret = "YOUR_CLIENT_SECRET";
using var httpClient = new HttpClient();

using var client = new AirthingsClient(httpClient, clientId, clientSecret);

var accounts = await client.ListAccounts();
// ...

```

## Documentation
- **[Complete Documentation](https://tudormobile.github.io/airthings/)** - Comprehensive guides and API reference
- **[API Documentation](https://tudormobile.github.io/airthings/api/Tudormobile.html)** - Generated API reference

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/tudormobile/Airthings/blob/main/LICENSE.txt) file for details.

