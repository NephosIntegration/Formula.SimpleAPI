# Formula.SimpleAPI
Easy API's for .Net

## Constrainable Resources
One of the main goals of this project was to provide an easy way to expose "resources" in a database through a RESTful API.  This is accomplished by building upon the [Formula.SimpleRepo](https://github.com/NephosIntegration/Formula.SimpleRepo) project.

In addition to common CRUD operations against resources, you can easily perform queries through a JSON expressions.

Example..
If you have a resource that has a column called Color, from the REST query endpoint you can constrain results by the color red.

```json
{color:'red'}
```

The items that you can constrain by aren't limited to columns, but can by dynamic attributes you can define on your repository.  This allows you to define how business concepts translate to constrainable concepts on your models.

```json
{color:'red',highAvailability:true}
```

In order to expose a resource, you must define a model and repository *(see [Formula.SimpleRepo](https://github.com/NephosIntegration/Formula.SimpleRepo) )*
Then create a Resource Controller

```c#
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Formula.SimpleAPI;
using MyApi.Data.Repositories;
using MyApi.Data.Models;

namespace MyApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TodoController : ResourceControllerBase<TodoController, Todo, TodoRepository>
    {
        public TodoController(ILogger<TodoController> logger, TodoRepository repository) : base(logger, repository)
        {
        }
    }
}
```

This controller alone provides the following fetch type routes

* GetList - /todo = Returns all Todo records
* Get - /todo/1 = Returns the Todo record with the id of 1
* Query - /todo/query/{'type':'chores','deleted':false} = would return all todo items that are of type chore that have not been deleted

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
