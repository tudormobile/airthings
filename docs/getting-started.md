# Getting Started

### Installation
```bash
dotnet add package Tudormobile.Airthings

-or-

dotnet add package Tudormobile.Airthings.Service
```

### Prerequisites
- **.NET 10.0** or later
- **Airthings client Id and client secret Key**
- **HttpClient** - For making HTTP requests

### Quick Start

#### Basic API Usage:
```cs
using Tudormobile.Airthings;

// Create an instance of HttpClient to be used for making API requests.
using var httpClient = new HttpClient();

// Replace "your_client_id" and "your_client_secret" with your actual respective id and secret.
using var client = new AirthingsClient(httpClient, "your_client_id", "your_client_secret");
