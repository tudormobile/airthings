## Airthings Library Namespaces
The namespaces available in the *Tudormobile.Airthings* library are described below.

### Tudormobile.Airthings.dll

```cs
using Tudormobile.Airthings;
```

- [Tudormobile.Airthings](Tudormobile.Airthings.yml)
    - Root namespace for the library. Essential classes that comprise the library.
    - Provides client and data model for directly accessing the Airthings Api.
    - Suitable for applications that directly access the Airthings Api.
```cs
using Tudormobile.Airthings.Proxy;
```
- [Tudormobile.Airthings.Proxy](Tudormobile.Airthings.Proxy.yml)
    - Proxy client interface, client, and response
    - Provides client to access the Airthings Api through a proxy service (see below)
    - Suitable for dotnet client side applications accessing a single Airthings registered application
    - Does not require client idendifier or client secret
    - Aggragated data response for all accounts and devices registered to the Airthings client identifier.

### AirthingsService.dll

```cs
using Tudormobile.Airthings.Service;
```
- [Tudormobile.Airthings.Service](Tudormobile.Airthings.Service.yml)
    - Proxy service for accessing the Airthings Api.  
    - Simplified data model for accessing Airthings devices.
    - Suitable for consumption by public front-end applications.

Latest unit testing results are shown below.
[!include[summary](../../src/output/SummaryGithub.md)]