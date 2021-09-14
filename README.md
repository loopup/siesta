# Siesta

Siesta is a generic C# REST client that allows you to more reliably consume a RESTful API that you own from another .NET environment.

## Technical overview

Siesta is 2 .netstandard2.1 NuGet packages that include classes for the configuration of requests and an HTTP client to consume Siesta configures APIs.

Siesta is split into a `Configuration` NuGet package, which includes everything you need to configure your requests and API, and a `CLient` package which includes what is needed to make calls to a Siesta configured API.

## Usage

Siesta is focused fully on DTOs (Data Transfer Objects).

Every controller endpoint has a corresponding DTO, even if this endpoint doesn't need to receive any data.

A Siesta DTO configures how to send a request and what form that data is expected back in (if at all).

Here we show some example controllers and how to configure requests.

###Get specific resource

This endpoint returns a `CustomerDto` but the controller wraps it in a `DataEnvelope`.

```c#
[HttpGet("/v1/customers/{id}")]
[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CustomerDto))]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<IActionResult> GetCustomer(Guid id)
{
    // Get customer
    return customerDto;
}
```

Now we can use Siesta to configure a request. As part of this request we can specify the Resource and also the Expected Return type.

In this case we will say that that the expected return type is the resource wrapped in a `DataEnvelope`. But you could have the resource as the return type.

You can provide a method to extract the resource from the return type.

```c#
public class GetCustomerRequest : SiestaRequestBase<CustomerDto, DataEnvelope<CustomerDto>>
{
    public Guid Id { get; set; }

    public override HttpRequestMessage GenerateRequestMessage()
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"v1/customers/{this.Id}"),
        };
        
        return request;
    }
    
    public override CustomerDto ExtractResourceFromReturn(DataEnvelope<CustomerDto> returnObject)
    {
        return returnObject.Data;
    }
}
```

Then a consumer can simply use this with the Siesta client like so:

```c#
public async Task<CustomerDto> GetCustomer(Guid id)
{
    var request = new GetCustomerRequest
    {
        Id = id,
    };
    
    var response = await this.siestaClient.SendAsync<GetCustomerRequest>(request);
    
    var customerDto = request.ExtractResourceFromReturn(response);
    
    return customerDto;
}
```

###Get multiple of a resource

This endpoint returns a `SerializablePagedList<CustomerDto>`.

```c#
[HttpGet("/v1/customers")]
[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SerializablePagedList<CustomerDto>))]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<IActionResult> GetCustomer([FromQuery] CustomerFilterInformation filterInformation)
{
    // Get customers using the filter information
    return customerDtos;
}
```

The `CustomerFilterInformation` looks something like this

```c#
public class CustomerFilterInformation : EnumerableFilterInformation
{
    public string Name { get; set; }
}
```

Now we can use Siesta to configure a request

```c#
public class GetCustomersRequest : SiestaRequestBase<DeserializedPagedList<CustomerDto>>
{
    public CustomerFilterInformation FilterInformation { get; set; }

    public override HttpRequestMessage GenerateRequestMessage()
    {
        var queryString = QueryHelpers.AddQueryString($"v1/customers", this.FilterInformation.AsQueryDictionary())
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(queryString),
        };
        
        return request;
    }
}
```

Then a consumer can simply use this with the Siesta client like so:

```c#
public async Task<DeserializedPagedList<CustomerDto>> GetCustomers(string name)
{
    var request = new GetCustomerRequest
    {
        FilterInformation = new CustomerFilterInformation
        {
            Name = "My great customer",
        },
    };
    
    var customerDtos = await this.siestaClient.SendAsync<GetCustomerRequest>(request);
    
    return customerDtos;
}
```

###Create a resource

This endpoint returns a `CustomerDto`.

```c#
[HttpPost("/v1/customers")]
[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CustomerDto))]
public async Task<IActionResult> GetCustomer([FromBody] CreateCustomerDto createCustomerDto)
{
    // Use the dto to create a customer
    return customerDto;
}
```

Now we can use Siesta to configure a request

```c#
public class CreateCustomersRequest : SiestaRequestBase<CustomerDto>
{
    public CreateCustomerDto CreateCustoemrDto { get; set; }

    public override HttpRequestMessage GenerateRequestMessage()
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri("v1/customers"),
            Body = JsonConvert.SerializeObject(this.CreateCustomerDto),
        };
        
        return request;
    }
}
```

Then a consumer can simply use this with the Siesta client like so:

```c#
public async Task<DeserializedPagedList<CustomerDto>> GetCustomers(string name)
{
    var request = new GetCustomerRequest
    {
        CreateCustomerDto = new CreateCustomerDto
        {
            ...
        },
    };
    
    var customerDtos = await this.siestaClient.SendAsync<GetCustomerRequest>(request);
    
    return customerDtos;
}
```

### Patch request

PATCH requests are rather special and challenging. Siesta provides a special request class for PATCH requests. It works based on the idea of having the modified version of a resource and then the original version provided to it. You therefore have to implement both a GET for the original resource and the PATCH request.

Siesta also provides a helper class for `JsonPatchDocument`. This class contains a method for generating a PATCH document by comparing two versions of a resource. This method is not perfect. It will handle nested properties and hierarchy.

It will also handle moving, adding or removing items in anything that implements `IList<T>`. However, it will not do nesting within an `IList<T>`. Therefore, for this to work properly anything complex inside an `IList` needs to implement comparisons, otherwise the PATCH will think every item in the list has changed, which may or may not cause issues.

It is recommended that you have a separate project to contain DTOs and Siesta requests that can be deployed as a Nuget package and consumed by any consumers of your API.

Hopefully it can be seen that this gives you a much greater control over how your API is controlled. You can fully unit test your requests and the effort for making calls accurately only has to happen once!

### Using a Siesta client you have been provided with

A Siesta client is perfectly primed to be injected as part of your DI provider at startup.

You can do all configuration of you Siesta client yourself. Simply register it how you would register any other HttpClient[https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-5.0#consumption-patterns].

However there are also some (currently one) extension methods to make registration easier. If there are some common use cases we will look to add these in future. If you think you have a common usage then please feel free to make a PR!

#### IServiceCollection extensions

You can use the `AddSiestaClientWithCorrelationIdAndSerilog` extension method to add a Siesta client that will log requests with Serilog and that will add a correlation id header to each request.

The current functionality includes:
- Including a correlation id with each request if one has been set on the request
- Logs info for every request made with System and machine information
- Logs all failed requests as an error

