# Siesta

Siesta is a generic C# REST client that allows you to more reliably consume a RESTful API that you own from another .NET environment.

## Technical overview

Siesta is a .netstandard2.1 Nuget package that includes classes for the configuration of requests and an HTTP client to consume Siesta configures APIs.

## Usage

Siesta is focused fully on DTOs (Data Transfer Objects).

Every controller endpoint has a corresponding DTO, even if this endpoint doesn't need to receive any data.

A Siesta DTO configures how to send a request and what form that data is expected back in (if at all).

Here we show some example controllers and how to configure requests.

###Get specific resource

This endpoint returns a `CustomerDto`.

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

Now we can use Siesta to configure a request

```c#
public class GetCustomerRequest : SiestaRequestBase<CustomerDto>
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
    
    var customerDto = await this.siestaClient.SendAsync<GetCustomerRequest>(request);
    
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
    
    var customerDto = await this.siestaClient.SendAsync<GetCustomerRequest>(request);
    
    return customerDto;
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
    
    var customerDto = await this.siestaClient.SendAsync<GetCustomerRequest>(request);
    
    return customerDto;
}
```

It is recommended that you have a separate project to contain DTOs and Siesta requests that can be deployed as a Nuget package and consumed by any consumers of your API.

Hopefully it can be seen that this gives you a much greater control over how your API is controlled. You can fully unit test your requests and the effort for making calls accurately only has to happen once!
