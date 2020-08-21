# Formula.SimpleAPI
Easy API's for .Net

# Constrainable Resources
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

# CORS Support via config file
A simple wrapper for CORS support can be supplied by a config file as a wrapper around the standard .net core utilities, allowing you to load your configuration from a config file.

To implement, modify your Startup.cs to include the following.

Add the following using;

```c#
using Formula.SimpleAPI;
```

## Startup.cs - ConfigureServices

Some extension methods have been provided for you register your configuration.
Within the **ConfigureServices** function of **Startup.cs** you can call **services.AddSimpleCors** providing it with an implementation of **ICorsConfig**.  

This can be done by creating your own class that implements the ICorsConfig contract, manually, however a more common way to provide configuration is via a JSON configuration file within the project using the CorsConfigDefinition.

CoresConfigDefition with origins as null will allow CORS from any origin, otherwise, an array of origins may be proviced.

```c#
services.AddSimpleCors(CorsConfigLoader.Get("corsConfig.json"));
```

You may also provide some defaults using a delegate.

```c#
// Load CORS config from file first, then via custom means
services.AddSimpleCors(CorsConfigLoader.Get("corsConfig.json", () =>
{
    var def = new CorsConfigDefinition();
    def.Origins = null; // By default, null origins means any origin

    // See if we have have been given instructions from appsettings 
    // or passed into us from the environment
    var corsOrigins = Configuration.GetSection("CorsOrigins").Value;
    if (String.IsNullOrWhiteSpace(corsOrigins) == false)
    {
        def.Origins = corsOrigins.Split(',');
    }

    return def;
}));
```
> This allows us to load our CORS Origins through either 1 of 3 options, via a `corsConfig.json`, via `appSettings.json` through a `CorsConfig` section, or finally via the environment (via k8s deployment). *(See `ConfigLoader` in [Formula.SimpleCore](https://github.com/NephosIntegration/Formula.SimpleCore) for details on how this functionality may be leveraged for other task)*

## Startup.cs - Configure

In the configure section of your app, you may call;

```c#
app.UseSimpleCors();
```

## Model State Errors

Good API's should guard data, return verbose information for failures (of all kinds, bad data or exceptions).  To help with this, SimpleAPI provides mechanisms for returning a consistent `envelope` of data on errors or bad data.

We want to be able to be able to validate our data models in order to sanitize user input via annotations on our models (see [here](https://docs.microsoft.com/en-us/aspnet/web-api/overview/formats-and-model-binding/model-validation-in-aspnet-web-api)).

If models passed to our API aren't valid, we want a validation error returned (see [Validation failure error response](https://docs.microsoft.com/en-us/aspnet/core/web-api/handle-errors?view=aspnetcore-3.1#validation-failure-error-response)).   All of our API's should return rich contextual error responses with a common syntax to them.  If they all have a common syntax, we can supply enough information such that our failure response, when a model is invalid, can be used to present useful information to a user (by pro-grammatically highlighting bad fields etc..).

In your `Startup.cs` add the following to the `ConfigureServices` function.

```c#
services.AddSimpleModelValidation();
```

You can now decorate models with annotations;

```c#
using System;
using System.ComponentModel.DataAnnotations;

namespace MyApi.Models
{
    public class HelloWorldModel
    {
        [System.ComponentModel.DataAnnotations.Required]
        [StringLength(10)]
        public string Name { get; set; }

        public string Message { get; set; }
    }
}
```

And create a controller that uses the model.  If the model isn't valid, or if an error occurrs, the response will be wrapped with useful, consistent information.

```c#
using System;
using MyApi.Models;
using Formula.SimpleCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

#pragma warning disable 1591
namespace MyApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HelloWorldController : ControllerBase
    {
        private readonly ILogger<HelloWorldController> _logger;

        public HelloWorldController(ILogger<HelloWorldController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public TypedStatusBuilder<HelloWorldModel> Post(HelloWorldModel model)
        {
            var output = new TypedStatusBuilder<HelloWorldModel>();
            try
            {
                var message = new HelloWorldModel();

                // Passing the name of "fail" will throw an error
                if ("fail".Equals(model.Name.ToLower()))
                {
                    int i = 0;
                    int j = 1;
                    int k = j / i;

                    message.Message = $"Hello {model.Name} did you know that {j} / {i} = {k}";
                }
                // Else give a normal response
                else
                {
                    message.Message = $"Hello {model.Name}!";
                }
                output.SetData(message);
            }
            catch (Exception ex)
            {
                output.RecordFailure(ex.Message);
            }
            return output;
        }
    }
}
```

You API's will start returning consistent data such as;

```json
{
  "isSuccessful": true,
  "message": null,
  "data": {
    "name": null,
    "message": "Hello World!"
  },
  "details": null
}
```

or 

```json
{
  "isSuccessful": false,
  "message": "One or more validation errors occurred.",
  "data": "One or more validation errors occurred.",
  "details": {
    "Name": "The field Name must be a string with a maximum length of 10."
  }
}
```

Example curl statements to test the various scenarios;

```bash
# This produces a successful response
curl -d '{"name":"World"}' -H 'Content-Type: application/json' -X POST http://localhost:5000/HelloWorld | jq

# This produces a failure response with details because it's too long.
curl -d '{"name":"ABCDEFGHIJKLMNOPQRSTUVWXYZ"}' -H 'Content-Type: application/json' -X POST http://localhost:5000/HelloWorld | jq

# This produces a failure response because expected data wasn't provided.
curl -d '{"name":""}' -H 'Content-Type: application/json' -X POST http://localhost:5000/HelloWorld | jq

# This is an example of what an in app exception looks like
curl -d '{"name":"fail"}' -H 'Content-Type: application/json' -X POST http://localhost:5000/HelloWorld | jq
```

# Packages / Projects Used
- [Microsoft.AspNetCore.Mvc](https://github.com/aspnet/mvc)
- [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)
- [Formula.SimpleRepo](https://github.com/NephosIntegration/Formula.SimpleRepo)
