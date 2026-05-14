# Airthings
Client access and object model for the Airthings Comsumer API.

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
