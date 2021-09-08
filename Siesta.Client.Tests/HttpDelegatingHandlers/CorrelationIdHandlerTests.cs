namespace Siesta.Client.Tests.HttpDelegatingHandlers
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Siesta.Client.HttpDelegatingHandlers;
    using Xunit;

    public class CorrelationIdHandlerTests
    {
        private HttpMessageInvoker messageInvoker = null!;

        [Fact]
        public async Task SendAsync_Always_ReturnsResponse()
        {
            var responseMessage = new HttpResponseMessage();
            this.SetUpInvoker(responseMessage);

            var result = await this.messageInvoker.SendAsync(new HttpRequestMessage(), CancellationToken.None);

            result.Should().Be(responseMessage);
        }

        [Fact]
        public async Task SendAsync_CorrelationIdHeaderNotPresent_AddsNewHeaderWithGuidId()
        {
            var correlationIdHeader = "X-Header";
            var responseMessage = new HttpResponseMessage();
            this.SetUpInvoker(responseMessage, correlationIdHeader);

            var result = await this.messageInvoker.SendAsync(new HttpRequestMessage(), CancellationToken.None);

            result.RequestMessage!.Headers.Contains(correlationIdHeader).Should().BeTrue();
            Guid.TryParse(
                result.RequestMessage!
                .Headers
                .GetValues(correlationIdHeader)
                .FirstOrDefault(),
                out _);
        }

        private void SetUpInvoker(HttpResponseMessage? responseMessage = null, string correlationIdHeader = "X-Correlation-Id")
        {
            var handler = new CorrelationIdHandler(correlationIdHeader)
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
                this.responseMessage.RequestMessage = request;
                return Task.FromResult(this.responseMessage);
            }
        }
    }
}
