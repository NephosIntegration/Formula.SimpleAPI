# Formula.SimpleAPI
Easy API's for .Net

## CORS Support via config file
A simple wrapper for CORS support can be supplied by a config file as a wrapper around the standard .net core utilities, allowing you to load your configuration from a config file.

To implement, modify your Startup.cs to include the following.

Add the following using;

```c#
using Formula.SimpleResourceServer;
```

## Startup.cs - ConfigureServices

Some extension methods have been provided for you register your configuration.
Within the **ConfigureServices** function of **Startup.cs** you can call **services.AddSimpleCors** providing it with an implementation of **ICorsConfig**.  

This can be done by creating your own class that implements the ICorsConfig contract, manually, however a more common way to provide configuration is via a JSON configuration file within the project using the CorsConfigDefinition.

CoresConfigDefition with origins as null will allow CORS from any origin, otherwise, an array of origins may be proviced.

```c#
services.AddSimpleCors(CorsConfigDefinition.Get("corsConfig.json"));
```

You may also provide some defaults using a delegate.

```c#
services.AddSimpleCors(CorsConfigDefinition.Get("corsConfig.json", () =>
{
    var def = new CorsConfigDefinition();
    def.Origins = null; // All from any origin right now
    return def;
}));
```

*(See ConfigLoader in Formula.Core for details on how this functionality may be leverage for other task)*

## Startup.cs - Configure

In the configure section of your app, you may call;

```c#
app.UseSimpleCors();
```

# Packages / Projects Used
- [Microsoft.AspNetCore.Mvc](https://github.com/aspnet/mvc)
- [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)
- [Formula.SimpleRepo](https://github.com/NephosIntegration/Formula.SimpleRepo)
