# Getting Started

### Installation
```bash
dotnet add package Tudormobile.Airthings

-or-

dotnet add package Tudormobile.Airthings.Service
```

### Prerequisites
- **.NET 10.0** or later
- **Airthings API Key**
- **HttpClient** - For making HTTP requests

### Quick Start

#### Basic API Usage:
```cs
using Tudormobile.Airthings;

// Create an instance of HttpClient to be used for making API requests.
using var httpClient = new HttpClient();

// Replace "your_api_key" with your actual API key for the financial data service.
using var client = new AirthingsClient(httpClient, "your_client_id", "your_client_secret");
