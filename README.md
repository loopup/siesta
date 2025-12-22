<p align="center">
  <img src="https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET"/>
  <img src="https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white" alt="C#"/>
  <img src="https://img.shields.io/badge/NuGet-004880?style=for-the-badge&logo=nuget&logoColor=white" alt="NuGet"/>
  <img src="https://img.shields.io/badge/License-MIT-yellow.svg?style=for-the-badge" alt="License: MIT"/>
</p>

<h1 align="center">🛋️ Siesta</h1>

<p align="center">
  <strong>A type-safe, DTO-driven REST client for .NET that makes API consumption a breeze.</strong>
</p>

<p align="center">
  <em>Stop writing boilerplate HTTP code. Start taking a Siesta.</em>
</p>

---

## ✨ Why Siesta?

Building reliable API integrations shouldn't be painful. Siesta provides a **strongly-typed**, **testable**, and **maintainable** approach to consuming RESTful APIs in .NET applications.

| Feature | Benefit |
|---------|---------|
| 🎯 **DTO-Driven Design** | Every endpoint has a corresponding request class—no more magic strings |
| 🔒 **Type Safety** | Compile-time checking for request/response types |
| 🧪 **Fully Testable** | Unit test your API requests without hitting real endpoints |
| 📦 **NuGet Ready** | Ship your API client as a package for consumers |
| 🔄 **Smart PATCH Support** | Automatic diff generation for partial updates |
| 📊 **Built-in Pagination** | First-class support for paged list responses |
| 🔗 **Correlation IDs** | Track requests across distributed systems |
| 📝 **Serilog Integration** | Comprehensive request/response logging out of the box |

---

## 📦 Installation

Siesta is distributed as two NuGet packages to support different use cases:

```bash
# For API providers - configure your endpoints
dotnet add package Siesta.Configuration

# For API consumers - make HTTP calls
dotnet add package Siesta.Client
```

| Package | Purpose | Target |
|---------|---------|--------|
| `Siesta.Configuration` | Request configuration, DTOs, serialization helpers | API providers & consumers |
| `Siesta.Client` | HTTP client, DI extensions, logging handlers | API consumers |

> **Compatibility:** .NET Standard 2.0+ and .NET 6.0+

---

## 🚀 Quick Start

### 1. Define Your Request

```csharp
public class GetCustomerRequest : SiestaRequest<CustomerDto, DataEnvelope<CustomerDto>>
{
    public Guid Id { get; set; }

    public override HttpRequestMessage GenerateRequestMessage()
    {
        return new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"v1/customers/{Id}", UriKind.Relative),
        };
    }
    
    public override CustomerDto ExtractResourceFromReturn(DataEnvelope<CustomerDto> returnObject)
    {
        return returnObject.Data;
    }
}
```

### 2. Register the Client

```csharp
services.AddSiestaClient<MyApiClient>(new CorrelationAndLoggingConfigurationOptions(
    baseAddress: new Uri("https://api.example.com"),
    systemName: "MyApplication"
));
```

### 3. Make the Call

```csharp
public class CustomerService
{
    private readonly ISiestaClient _siestaClient;

    public CustomerService(ISiestaClient siestaClient)
    {
        _siestaClient = siestaClient;
    }

    public async Task<CustomerDto> GetCustomerAsync(Guid id)
    {
        var request = new GetCustomerRequest { Id = id };
        var response = await _siestaClient.SendAsync(request);
        return request.ExtractResourceFromReturn(response);
    }
}
```

---

## 📖 Detailed Usage Guide

### Request Types

Siesta provides three base request classes for different scenarios:

| Base Class | Use Case | Returns Data? |
|------------|----------|---------------|
| `SiestaRequest` | DELETE, fire-and-forget operations | ❌ No |
| `SiestaRequest<TResource, TReturn>` | GET, POST, PUT operations | ✅ Yes |
| `SiestaPatchRequest<TReturn, TResource, TGetReturn>` | PATCH operations with diff | ✅ Yes |

---

### 📥 GET Single Resource

**Controller:**
```csharp
[HttpGet("/v1/customers/{id}")]
[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DataEnvelope<CustomerDto>))]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<IActionResult> GetCustomer(Guid id)
{
    var customer = await _customerService.GetByIdAsync(id);
    return Ok(new DataEnvelope<CustomerDto> { Data = customer });
}
```

**Request Configuration:**
```csharp
public class GetCustomerRequest : SiestaRequest<CustomerDto, DataEnvelope<CustomerDto>>
{
    public Guid Id { get; set; }

    public override HttpRequestMessage GenerateRequestMessage()
    {
        return new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"v1/customers/{Id}", UriKind.Relative),
        };
    }
    
    public override CustomerDto ExtractResourceFromReturn(DataEnvelope<CustomerDto> returnObject)
    {
        return returnObject.Data;
    }
}
```

**Consumer Usage:**
```csharp
var request = new GetCustomerRequest { Id = customerId };
var response = await _siestaClient.SendAsync(request);
var customer = request.ExtractResourceFromReturn(response);
```

---

### 📋 GET Collection with Pagination & Filtering

**Filter Information:**
```csharp
public class CustomerFilterInformation : EnumerableFilterInformation
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public DateTime? CreatedAfter { get; set; }
}
```

**Controller:**
```csharp
[HttpGet("/v1/customers")]
[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SerializablePagedList<CustomerDto>))]
public async Task<IActionResult> GetCustomers([FromQuery] CustomerFilterInformation filter)
{
    var customers = await _customerService.GetPagedAsync(filter);
    return Ok(new SerializablePagedList<CustomerDto>(
        customers.Items, 
        filter.PageNumber, 
        filter.PageSize, 
        customers.TotalCount
    ));
}
```

**Request Configuration:**
```csharp
public class GetCustomersRequest : SiestaRequest<DeserializedPagedList<CustomerDto>, DeserializedPagedList<CustomerDto>>
{
    public CustomerFilterInformation FilterInformation { get; set; } = new();

    public override HttpRequestMessage GenerateRequestMessage()
    {
        var queryString = QueryHelpers.AddQueryString(
            "v1/customers", 
            FilterInformation.AsQueryDictionary()
        );
        
        return new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(queryString, UriKind.Relative),
        };
    }
    
    public override DeserializedPagedList<CustomerDto> ExtractResourceFromReturn(
        DeserializedPagedList<CustomerDto> returnObject) => returnObject;
}
```

**Consumer Usage:**
```csharp
var request = new GetCustomersRequest
{
    FilterInformation = new CustomerFilterInformation
    {
        Name = "Acme",
        PageNumber = 1,
        PageSize = 25
    }
};

var pagedCustomers = await _siestaClient.SendAsync(request);

Console.WriteLine($"Page {pagedCustomers.PageNumber} of {pagedCustomers.PageCount}");
Console.WriteLine($"Total: {pagedCustomers.TotalItemCount} customers");

foreach (var customer in pagedCustomers.Items)
{
    Console.WriteLine($"- {customer.Name}");
}
```

---

### ➕ POST Create Resource

**Controller:**
```csharp
[HttpPost("/v1/customers")]
[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CustomerDto))]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerDto dto)
{
    var customer = await _customerService.CreateAsync(dto);
    return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
}
```

**Request Configuration:**
```csharp
public class CreateCustomerRequest : SiestaRequest<CustomerDto, CustomerDto>
{
    public CreateCustomerDto CreateCustomerDto { get; set; } = null!;

    public override HttpRequestMessage GenerateRequestMessage()
    {
        return new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri("v1/customers", UriKind.Relative),
            Content = new StringContent(
                JsonConvert.SerializeObject(CreateCustomerDto),
                Encoding.UTF8,
                "application/json"
            ),
        };
    }
    
    public override CustomerDto ExtractResourceFromReturn(CustomerDto returnObject) => returnObject;
}
```

**Consumer Usage:**
```csharp
var request = new CreateCustomerRequest
{
    CreateCustomerDto = new CreateCustomerDto
    {
        Name = "New Customer",
        Email = "customer@example.com"
    }
};

var createdCustomer = await _siestaClient.SendAsync(request);
Console.WriteLine($"Created customer with ID: {createdCustomer.Id}");
```

---

### 🔄 PATCH Update Resource (Smart Diff)

Siesta's PATCH support is powerful—it automatically generates a JSON Patch document by comparing the original and modified versions of a resource.

**Request Configuration:**
```csharp
public class UpdateCustomerRequest : SiestaPatchRequest<CustomerDto, CustomerDto, DataEnvelope<CustomerDto>>
{
    public Guid Id { get; set; }
    public CustomerDto ModifiedCustomer { get; set; } = null!;

    public override HttpRequestMessage GenerateGetRequestMessage()
    {
        return new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"v1/customers/{Id}", UriKind.Relative),
        };
    }

    public override CustomerDto ExtractResourceFromGetReturn(DataEnvelope<CustomerDto> getReturn)
    {
        return getReturn.Data;
    }

    public override HttpRequestMessage GeneratePatchRequestMessage(CustomerDto originalResource)
    {
        var patchDocument = JsonPatchDocumentHelpers.GeneratePatchDocument(
            originalResource, 
            ModifiedCustomer
        );

        return new HttpRequestMessage
        {
            Method = HttpMethod.Patch,
            RequestUri = new Uri($"v1/customers/{Id}", UriKind.Relative),
            Content = new StringContent(
                JsonConvert.SerializeObject(patchDocument.Operations),
                Encoding.UTF8,
                "application/json-patch+json"
            ),
        };
    }
}
```

**Consumer Usage:**
```csharp
// Fetch, modify, and patch in one operation
var request = new UpdateCustomerRequest
{
    Id = customerId,
    ModifiedCustomer = new CustomerDto
    {
        Id = customerId,
        Name = "Updated Name",  // Changed
        Email = existingEmail   // Unchanged - won't be in patch
    }
};

var updatedCustomer = await _siestaClient.SendAsync(request);
```

> **💡 How it works:** Siesta automatically:
> 1. Fetches the current resource via GET
> 2. Compares it with your modified version
> 3. Generates a minimal JSON Patch document
> 4. Sends only the changes via PATCH

---

### 🗑️ DELETE Resource

**Request Configuration:**
```csharp
public class DeleteCustomerRequest : SiestaRequest
{
    public Guid Id { get; set; }

    public override HttpRequestMessage GenerateRequestMessage()
    {
        return new HttpRequestMessage
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri($"v1/customers/{Id}", UriKind.Relative),
        };
    }
}
```

**Consumer Usage:**
```csharp
var request = new DeleteCustomerRequest { Id = customerId };
await _siestaClient.SendAsync(request);
```

---

## ⚙️ Configuration

### Dependency Injection Setup

Siesta provides a convenient extension method for registering clients with built-in correlation ID tracking and Serilog logging:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddSiestaClient<MyApiClient>(new CorrelationAndLoggingConfigurationOptions(
        baseAddress: new Uri("https://api.example.com"),
        systemName: "OrderService",
        defaultHeaders: new Dictionary<string, string>
        {
            ["X-Api-Version"] = "2.0",
            ["X-Client-Name"] = "MyApp"
        },
        authenticationHeaderValue: new AuthenticationHeaderValue("Bearer", GetAccessToken()),
        loggerCorrelationId: "CorrelationId",
        requestHeaderCorrelationIdKey: "X-Correlation-ID"
    ));
}
```

### Configuration Options

| Option | Description | Default |
|--------|-------------|---------|
| `BaseAddress` | Base URL for all API requests | *Required* |
| `SystemName` | Identifier for logging purposes | *Required* |
| `DefaultHeaders` | Headers added to every request | `{}` |
| `AuthenticationHeaderValue` | Auth header (e.g., Bearer token) | `null` |
| `LoggerCorrelationId` | Serilog property name for correlation ID | `"CorrelationId"` |
| `RequestHeaderCorrelationIdKey` | HTTP header name for correlation ID | `"X-Correlation-ID"` |

### Manual HttpClient Configuration

For advanced scenarios, you can configure the HttpClient yourself:

```csharp
services.AddHttpClient<ISiestaClient, SiestaClient>(client =>
{
    client.BaseAddress = new Uri("https://api.example.com");
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
})
.AddPolicyHandler(GetRetryPolicy())
.AddPolicyHandler(GetCircuitBreakerPolicy());
```

---

## 🔗 Correlation ID Support

Track requests across distributed systems by passing correlation IDs:

```csharp
// Pass an existing correlation ID to maintain traceability
var response = await _siestaClient.SendAsync(request, currentCorrelationId: "abc-123-xyz");
```

When using the built-in DI extension, correlation IDs are automatically:
- Added to outgoing request headers
- Logged with Serilog for each request
- Propagated through the request chain

---

## ⚠️ Exception Handling

Siesta provides specific exceptions for different failure scenarios:

```csharp
try
{
    var customer = await _siestaClient.SendAsync(request);
}
catch (SiestaHttpCallFailedException ex)
{
    // HTTP request failed (4xx, 5xx responses)
    Console.WriteLine($"Status: {ex.FailedHttpResponseMessage.StatusCode}");
    Console.WriteLine($"Response: {ex.Message}");
}
catch (SiestaContentException ex)
{
    // Response couldn't be deserialized
    Console.WriteLine("Invalid response format");
}
catch (SiestaConfigurationException ex)
{
    // Request configuration error
    Console.WriteLine($"Configuration error: {ex.Message}");
}
```

| Exception | Cause |
|-----------|-------|
| `SiestaHttpCallFailedException` | Non-success HTTP status code |
| `SiestaContentException` | Response deserialization failed |
| `SiestaConfigurationException` | Invalid request configuration |
| `SiestaRequestNotImplementedException` | Required method not overridden |

---

## 📦 Packaging Your API Client

**Best Practice:** Create a separate NuGet package containing your DTOs and Siesta request classes. This allows API consumers to simply install your package and start making calls.

```
MyApi.Client/
├── DTOs/
│   ├── CustomerDto.cs
│   └── CreateCustomerDto.cs
├── Requests/
│   ├── GetCustomerRequest.cs
│   ├── GetCustomersRequest.cs
│   ├── CreateCustomerRequest.cs
│   └── UpdateCustomerRequest.cs
├── Filters/
│   └── CustomerFilterInformation.cs
└── MyApiClient.cs
```

**MyApiClient.cs:**
```csharp
public class MyApiClient : SiestaClient
{
    public MyApiClient(HttpClient httpClient) : base(httpClient) { }
    
    public MyApiClient(HttpClient httpClient, SiestaClientConfigurationOptions options) 
        : base(httpClient, options) { }
}
```

Consumers can then use your client with minimal setup:

```csharp
services.AddSiestaClient<MyApiClient>(options);
```

---

## 🧪 Testing

Siesta's design makes testing straightforward:

### Unit Testing Requests

```csharp
[Fact]
public void GetCustomerRequest_GeneratesCorrectUri()
{
    // Arrange
    var request = new GetCustomerRequest 
    { 
        Id = Guid.Parse("12345678-1234-1234-1234-123456789012") 
    };

    // Act
    var httpRequest = request.GenerateRequestMessage();

    // Assert
    Assert.Equal(HttpMethod.Get, httpRequest.Method);
    Assert.Equal("v1/customers/12345678-1234-1234-1234-123456789012", httpRequest.RequestUri?.ToString());
}
```

### Integration Testing with Mocked HttpClient

```csharp
[Fact]
public async Task GetCustomer_ReturnsCustomer_WhenFound()
{
    // Arrange
    var expectedCustomer = new CustomerDto { Id = Guid.NewGuid(), Name = "Test" };
    var mockHandler = new MockHttpMessageHandler(new HttpResponseMessage
    {
        StatusCode = HttpStatusCode.OK,
        Content = new StringContent(JsonConvert.SerializeObject(
            new DataEnvelope<CustomerDto> { Data = expectedCustomer }
        ))
    });
    
    var client = new SiestaClient(new HttpClient(mockHandler) 
    { 
        BaseAddress = new Uri("https://api.test.com") 
    });

    // Act
    var request = new GetCustomerRequest { Id = expectedCustomer.Id };
    var response = await client.SendAsync(request);
    var result = request.ExtractResourceFromReturn(response);

    // Assert
    Assert.Equal(expectedCustomer.Name, result.Name);
}
```

---

## 🤝 Contributing

We welcome contributions! Here's how to get started:

1. **Fork** the repository
2. **Create** a feature branch (`git checkout -b feature/amazing-feature`)
3. **Commit** your changes (`git commit -m 'feat: add amazing feature'`)
4. **Push** to the branch (`git push origin feature/amazing-feature`)
5. **Open** a Pull Request

Please ensure your PR:
- Follows the existing code style
- Includes appropriate tests
- Updates documentation as needed

See [CHANGELOG.md](CHANGELOG.md) for version history.

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](MITLicense.txt) file for details.

---

<p align="center">
  <strong>Built with ☕ by the LoopUp team</strong>
</p>

<p align="center">
  <a href="https://github.com/loopup/siesta/issues">Report Bug</a>
  ·
  <a href="https://github.com/loopup/siesta/issues">Request Feature</a>
</p>
