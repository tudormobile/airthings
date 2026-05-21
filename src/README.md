# Service Applications

## Projects
- `Airthings` - Library package *Tudormobile.Airthings*
- `Airthings.Tests` - Unit tests for the library package
- `Airthings.IntegrationTests` - Integration tests for the library package  
- `Airthings.Service` - Web services application
- `Airthings.Service.Tests` - Unit tests for the web services application

## Build, Test, Package
```sh
dotnet build
dotnet test
```
> [!NOTE]
> Building a *Release* configuration generates nuget packages.
```sh
dotnet build -c Release
```
## Deployment
- `Tudormobile.Airthings.1.0.0.nupkg`
    - Client library for accessing the Airthings consumer API.
    - Use to build apps accessing Airthings services.
- `Tudormobile.Airthings.Service.1.0.0.nupkg`
    - Service library to expose the Airthings sensor data via proxy web services.
    - Proxy client to access the services via proxy.
