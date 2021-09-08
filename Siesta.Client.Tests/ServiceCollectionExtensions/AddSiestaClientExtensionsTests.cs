namespace Siesta.Client.Tests.ServiceCollectionExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using FluentAssertions;
    using Microsoft.Extensions.DependencyInjection;
    using Moq;
    using Serilog;
    using Siesta.Client.HttpDelegatingHandlers;
    using Siesta.Client.ServiceCollectionExtensions;
    using Xunit;

    public class AddSiestaClientExtensionsTests
    {
        private readonly IServiceCollection serviceCollection;
        private readonly Mock<ILogger> loggerMock;

        public AddSiestaClientExtensionsTests()
        {
            this.loggerMock = new Mock<ILogger>();
            this.serviceCollection = new ServiceCollection();
            this.serviceCollection.AddSingleton<ILogger>(sp => this.loggerMock.Object);
        }

        // These seem to be about as much as can be tested for this class unfortunately
        [Fact]
        public void AddSiestaClientWithCorrelationIdAndSerilog_Always_AddsSerilogHandler()
        {
            var systemName = "System name";
            var loggerCorrelationIdKey = "id";
            var requestHeaderCorrelationIdKey = "correlationId";
            var expectedHandler = new SerilogHandler(
                this.loggerMock.Object,
                systemName,
                loggerCorrelationIdKey,
                requestHeaderCorrelationIdKey);
            this.serviceCollection.AddSiestaClientWithCorrelationIdAndSerilog<TestClient>(
                new Uri("https://base.address.com"),
                systemName);

            var serviceProvider = this.serviceCollection.BuildServiceProvider();

            var handler = serviceProvider.GetService<SerilogHandler>();
            handler.Should().BeEquivalentTo(expectedHandler);
        }

        [Fact]
        public void AddSiestaClientWithCorrelationIdAndSerilog_Always_AddsCorrelationIdHandler()
        {
            var systemName = "System name";
            var requestHeaderCorrelationIdKey = "correlationId";
            var expectedHandler = new CorrelationIdHandler(requestHeaderCorrelationIdKey);
            this.serviceCollection.AddSiestaClientWithCorrelationIdAndSerilog<TestClient>(
                new Uri("https://base.address.com"),
                systemName);

            var serviceProvider = this.serviceCollection.BuildServiceProvider();

            var handler = serviceProvider.GetService<CorrelationIdHandler>();
            handler.Should().BeEquivalentTo(expectedHandler);
        }

        [Fact]
        public void AddSiestaClientWithCorrelationIdAndSerilog_NoCorrelationIdNamesProvided_AddsSerilogHandlerWithDefaults()
        {
            var systemName = "System name";
            var expectedHandler = new SerilogHandler(
                this.loggerMock.Object,
                systemName,
                "CorrelationId",
                "X-Correlation-ID");
            this.serviceCollection.AddSiestaClientWithCorrelationIdAndSerilog<TestClient>(
                new Uri("https://base.address.com"),
                systemName);

            var serviceProvider = this.serviceCollection.BuildServiceProvider();

            var handler = serviceProvider.GetService<SerilogHandler>();
            handler.Should().BeEquivalentTo(expectedHandler);
        }

        [Fact]
        public void AddSiestaClientWithCorrelationIdAndSerilog_Always_AddsClientWithBaseAddress()
        {
            var baseAddress = new Uri("https://base.address.com");
            var expectedClient = new HttpClient
            {
                BaseAddress = baseAddress,
            };

            this.serviceCollection.AddSiestaClientWithCorrelationIdAndSerilog<TestClient>(
                baseAddress,
                "System name");

            var serviceProvider = this.serviceCollection.BuildServiceProvider();

            var client = serviceProvider.GetRequiredService<TestClient>();
            client.Client.Should().BeEquivalentTo(expectedClient);
        }

        [Fact]
        public void AddSiestaClientWithCorrelationIdAndSerilog_DefaultHeadersProvided_AddsClientWithDefaultHeaders()
        {
            var defaultHeaders = new Dictionary<string, string>
            {
                { "Header1", "value1" },
                { "Header2", "value2" },
                { "Header3", "value3" },
            };
            var baseAddress = new Uri("https://base.address.com");
            var expectedClient = new HttpClient
            {
                BaseAddress = baseAddress,
            };
            foreach (var header in defaultHeaders)
            {
                expectedClient.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            this.serviceCollection.AddSiestaClientWithCorrelationIdAndSerilog<TestClient>(
                baseAddress,
                "System name",
                defaultHeaders);

            var serviceProvider = this.serviceCollection.BuildServiceProvider();

            var client = serviceProvider.GetRequiredService<TestClient>();
            client.Client.Should().BeEquivalentTo(expectedClient);
        }

        [Fact]
        public void AddSiestaClientWithCorrelationIdAndSerilog_AuthorizationHeaderValueProvided_AddsClientWithAuthorizationHeader()
        {
            var authenticationHeaderValue = new AuthenticationHeaderValue("Bearer", "token");
            var baseAddress = new Uri("https://base.address.com");
            var expectedClient = new HttpClient
            {
                BaseAddress = baseAddress,
            };
            expectedClient.DefaultRequestHeaders.Authorization = authenticationHeaderValue;

            this.serviceCollection.AddSiestaClientWithCorrelationIdAndSerilog<TestClient>(
                baseAddress,
                "System name",
                authenticationHeaderValue: authenticationHeaderValue);

            var serviceProvider = this.serviceCollection.BuildServiceProvider();

            var client = serviceProvider.GetRequiredService<TestClient>();
            client.Client.Should().BeEquivalentTo(expectedClient);
        }
    }

    internal class TestClient : SiestaClient
    {
        public TestClient(HttpClient httpClient, SiestaClientConfigurationOptions siestaClientConfigurationOptions)
            : base(httpClient, siestaClientConfigurationOptions)
        {
            this.Client = httpClient;
        }

        public HttpClient Client { get; }
    }
}
