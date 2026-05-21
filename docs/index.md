---
_layout: landing
---

# Tudormobile.Airthings

***Tudormobile.Airthings*** provides a C# SDK for accessing the Airthings consumer API.

## Documentation

- **[Introduction](introduction.md)** - Architecture overview and roadmap  
- **[Getting Started](getting-started.md)** - Installation, examples, and best practices
- **[Generated API Docs](api/Tudormobile.md)** - Auto-generated technical reference  

## Quick Start

```cs
using Tudormobile.Airthings;

var clientId = "your_client_id";
var clientSecret = "your_client_secret";

using var httpClient = new HttpClient();
using var client = new AirthingsClient(httpClient, clientId, clientSecret);

var response = await client.ListAccounts();
if (response.IsSuccess)
{
    // ...
}
```
---

**Links:** [`Source Code`](https://github.com/tudormobile/airthings) | [`NuGet Package`](https://www.nuget.org/packages/Tudormobile.Airthings)
