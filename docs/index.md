---
_layout: landing
---

# Tudormobile.Airthings

***Tudormobile.Apithings*** provides a C# SDK for accessing the Airthings consumer API.

## Documentation

- **[Introduction](introduction.md)** - Architecture overview and roadmap  
- **[Getting Started](getting-started.md)** - Installation, examples, and best practices
- **[Generated API Docs](api/Tudormobile.md)** - Auto-generated technical reference  

## Quick Start

```cs
using Tudormobile.Airthings;

using var httpClient = new HttpClient();
using var client = new AirthingClient(httpClient, "your_client_id", "your_client_secret");

```

## Build Documentation

[Building the documentation](README.md) locally using DocFX.    

---

**Links:** [`Source Code`](https://github.com/tudormobile/airthings) | [`NuGet Package`](https://www.nuget.org/packages/Tudormobile.Airthings)
