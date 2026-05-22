# AIRTHINGS
[![Build and Deploy](https://github.com/tudormobile/airthings/actions/workflows/dotnet.yml/badge.svg)](https://github.com/tudormobile/airthings/actions/workflows/dotnet.yml)
[![Publish Docs](https://github.com/tudormobile/airthings/actions/workflows/docs.yml/badge.svg)](https://github.com/tudormobile/airthings/actions/workflows/docs.yml)  
![GitHub Issues or Pull Requests](https://img.shields.io/github/issues/tudormobile/airthings)  
[![NuGet](https://img.shields.io/nuget/v/Tudormobile.Airthings.Service.svg)](https://www.nuget.org/packages/Tudormobile.Airthings.Service/)
[![License](https://img.shields.io/github/license/tudormobile/Airthings)](https://github.com/tudormobile/Airthings/blob/main/LICENSE.txt)
[![.NET](https://img.shields.io/badge/.NET-10.0-blue.svg)](https://dotnet.microsoft.com/download)  

Airthings Consumer Api

## Overview
An Airthings client library, a proxy web service library, and a proxy client for accessing the Airthings consumer API using both ***dotnet*** and ***vue*** components. 

`/docs` - Documentation  
`/samples` - Sample proxy client applications  
`/src` - Dotnet source for base library and proxy service  
`/vue-airthings` - Vue component library  

### Releases
Production releases include nuget packages (2), VUE component (1), and zip archive as well as documentation deployed to [Github Pages](https://tudormobile.github.io/airthings/).

## Building Apps
In order to build apps with the Airthings Consumer API, you need to register the app on the Airthings website.
- `Client Id` = YOUR_CLIENT_ID
- `Client Secret` = YOUR_CLIENT_SECRET

## Building a Proxy Service
To build a proxy service, include the `Airthings.Service` library to expose the following endpoints:

/home/airthings/v1  
- `/status` - Display service status and version information
- `/devices` - Lists all available devices
- `/samples` - Latest data from all devices
- `/summary` - Summary data for all devices

All endpoints are cached for 30-60 minutes. Application identifier and client secret are used only in the proxy service configuration.

```cs
using Tudormobile.Airthings.Service;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Service(builder.Configuration);

var app = builder.Build();
app.UseAirthingsService();

app.Run();
```
## Proxy Clients

`samples\SimpleConsoleApp` - Proxy client sample in ***dotnet***.  
`samples\vue-app` - Proxy client sample in ***Vue***.