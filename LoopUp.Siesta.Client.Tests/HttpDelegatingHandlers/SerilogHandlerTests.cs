namespace LoopUp.Siesta.Client.Tests.HttpDelegatingHandlers
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using LoopUp.Siesta.Client.HttpDelegatingHandlers;
    using Serilog;
    using Serilog.Events;
    using Serilog.Sinks.TestCorrelator;
    using Xunit;

    public class SerilogHandlerTests : IDisposable
    {
        private readonly string systemName = "Siesta";
        private readonly ITestCorrelatorContext testCorrelatorContext;

        private HttpMessageInvoker messageInvoker = null!;

        public SerilogHandlerTests()
        {
            this.testCorrelatorContext = TestCorrelator.CreateContext();
            this.SetUpInvoker();
        }

        #region SendAsync

        [Fact]
        public async Task SendAsync_Always_LogsMethodBeingSent()
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri("https://some.api.com/v1/endpoint"),
            };

            await this.messageInvoker.SendAsync(request, CancellationToken.None);

            TestCorrelator
                .GetLogEventsFromCurrentContext()
                .Should()
                .ContainSingle()
                .Which
                .RenderMessage()
                .Should()
                .Be($"Sending \"{this.systemName}:{Environment.MachineName}\" request to \"{request.Method}\" {request.RequestUri}");
        }

        [Fact]
        public async Task SendAsync_CorrelationIdHeaderExists_LogsWithConfiguredCorrelationIdName()
        {
            var loggerCorrelationIdName = "CorrelationIdName";
            var requestHeaderCorrelationIdKey = "X-Correlation-ID";
            var correlationId = Guid.NewGuid().ToString();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri("https://some.api.com/v1/endpoint"),
            };
            request.Headers.Add(requestHeaderCorrelationIdKey, correlationId);

            this.SetUpInvoker(loggerCorrelationIdName, requestHeaderCorrelationIdKey);
            await this.messageInvoker.SendAsync(request, CancellationToken.None);

            TestCorrelator
                .GetLogEventsFromCurrentContext()
                .Should()
                .ContainSingle()
                .Which
                .Level
                .Should()
                .Be(LogEventLevel.Information);
            TestCorrelator
                .GetLogEventsFromCurrentContext()
                .Should()
                .ContainSingle()
                .Which
                .Properties[loggerCorrelationIdName]
                .Should()
                .Be(new ScalarValue(correlationId));
        }

        [Fact]
        public async Task SendAsync_ResponseIsUnsuccessfulAndCorrelationId_LogsError()
        {
            var correlationId = Guid.NewGuid().ToString();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri("https://some.api.com/v1/endpoint"),
            };
            request.Headers.Add("X-Correlation-ID", correlationId);
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("Response content"),
            };

            this.SetUpInvoker(responseMessage: response);
            await this.messageInvoker.SendAsync(request, CancellationToken.None);

            var errorEvent = TestCorrelator.GetLogEventsFromCurrentContext().ToList()[1];
            errorEvent.Level.Should().Be(LogEventLevel.Error);
            errorEvent
                .RenderMessage()
                .Should()
                .Be(string.Format(
                    "Request from \"{0}\" to \"{1}\" {2} failed with code {3} and response body \"{4}\"",
                    $"{this.systemName}:{Environment.MachineName}",
                    request.Method,
                    request.RequestUri,
                    response.StatusCode,
                    await response.Content.ReadAsStringAsync()));
            errorEvent
                .Properties["CorrelationId"]
                .Should()
                .Be(new ScalarValue(correlationId));
        }

        [Fact]
        public async Task SendAsync_ResponseIsUnsuccessfulNoCorrelationId_LogsError()
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri("https://some.api.com/v1/endpoint"),
            };
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("Response content"),
            };

            this.SetUpInvoker(responseMessage: response);
            await this.messageInvoker.SendAsync(request, CancellationToken.None);

            var errorEvent = TestCorrelator.GetLogEventsFromCurrentContext().ToList()[1];
            errorEvent.Level.Should().Be(LogEventLevel.Error);
            errorEvent
                .RenderMessage()
                .Should()
                .Be(string.Format(
                    "Request from \"{0}\" to \"{1}\" {2} failed with code {3} and response body \"{4}\"",
                    $"{this.systemName}:{Environment.MachineName}",
                    request.Method,
                    request.RequestUri,
                    response.StatusCode,
                    await response.Content.ReadAsStringAsync()));
        }

        [Fact]
        public async Task SendAsync_ResponseIsUnsuccessful_ReturnsResponse()
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri("https://some.api.com/v1/endpoint"),
            };
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("Response content"),
            };

            this.SetUpInvoker(responseMessage: response);
            var result = await this.messageInvoker.SendAsync(request, CancellationToken.None);

            result.Should().Be(response);
        }

        [Fact]
        public async Task SendAsync_ResponseIsSuccessful_ReturnsResponse()
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri("https://some.api.com/v1/endpoint"),
            };
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("Response content"),
            };

            this.SetUpInvoker(responseMessage: response);
            var result = await this.messageInvoker.SendAsync(request, CancellationToken.None);

            result.Should().Be(response);
        }

        #endregion

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.messageInvoker.Dispose();
                this.testCorrelatorContext.Dispose();
            }
        }

        private void SetUpInvoker(
            string? loggerCorrelationIdName = null,
            string? requestHeaderCorrelationIdKey = null,
            HttpResponseMessage? responseMessage = null)
        {
            var logger = new LoggerConfiguration().WriteTo.Sink(new TestCorrelatorSink()).Enrich.FromLogContext().CreateLogger();
            var handler = new SerilogHandler(
                logger,
                this.systemName,
                loggerCorrelationIdName ?? "CorrelationId",
                requestHeaderCorrelationIdKey ?? "X-Correlation-ID")
            {
                InnerHandler = new TestHandler(responseMessage ?? new HttpResponseMessage()),
            };

            this.messageInvoker = new HttpMessageInvoker(handler);
        }

        private class TestHandler : DelegatingHandler
        {
            private readonly HttpResponseMessage responseMessage;

            public TestHandler(HttpResponseMessage responseMessage) => this.responseMessage = responseMessage;

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return Task.FromResult(this.responseMessage);
            }
        }
    }
}
