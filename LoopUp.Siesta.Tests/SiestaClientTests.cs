using LoopUp.Siesta.RequestConfiguration;

namespace LoopUp.Siesta.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using LoopUp.Siesta.DtoConfiguration;
    using LoopUp.Siesta.Exceptions;
    using LoopUp.Siesta.Serialization;
    using Microsoft.AspNetCore.WebUtilities;
    using Moq;
    using Moq.Protected;
    using Newtonsoft.Json;
    using Xunit;

    public class SiestaClientTests
    {
        private readonly Mock<HttpMessageHandler> httpMessageHandlerMock;
        private readonly SiestaClient sut;

        public SiestaClientTests()
        {
            this.httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var httpClient = new HttpClient(this.httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("https://test.service.com"),
            };

            this.sut = new SiestaClient(httpClient);
        }

        #region SendAsync

        [Fact]
        public async Task SendAsync_HttpCallSuccessful_ReturnsContent()
        {
            var httpRequest = new HttpRequestMessage();
            var dto = new TestContentSiestaRequest(httpRequest);
            var responseContent = new TestContent
            {
                Id = dto.Id,
            };

            this.SetupMessageHandler(responseContent, httpRequest);

            var result = await this.sut.SendAsync(dto);

            result.Should().BeEquivalentTo(responseContent);
        }

        [Fact]
        public async Task SendAsync_NoContentExpectedHttpCallSuccessful_ReturnsCompletedTask()
        {
            var httpRequest = new HttpRequestMessage();
            var dto = new TestNoContentSiestaRequest(httpRequest);
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
            };

            this.SetupMessageHandler(responseMessage, httpRequest);

            var result = await this.sut.SendAsync(dto);

            result.IsCompleted.Should().BeTrue();
        }

        [Fact]
        public async Task SendAsync_HttpCallReturnsUnsuccessful_ThrowsSiestaHttpException()
        {
            var httpRequest = new HttpRequestMessage();
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
            };
            var dto = new TestContentSiestaRequest(httpRequest);

            this.SetupMessageHandler(responseMessage, httpRequest);

            var exception = await Assert.ThrowsAsync<SiestaHttpException>(() =>
                this.sut.SendAsync(dto));

            exception.FailedHttpResponseMessage.Should().Be(responseMessage);
            exception.Message.Should().Be("HTTP call was not successful.");
        }

        [Fact]
        public async Task SendAsync_HttpContentDoesNotMatchExpectedFormat_ThrowsSiestaHttpException()
        {
            var httpRequest = new HttpRequestMessage();
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("just a string"),
            };
            var dto = new TestContentSiestaRequest(httpRequest);

            this.SetupMessageHandler(responseMessage, httpRequest);

            var exception = await Assert.ThrowsAsync<SiestaHttpException>(() =>
                this.sut.SendAsync(dto));

            exception.FailedHttpResponseMessage.Should().Be(responseMessage);
            exception.Message.Should().Be("Content was not as expected.");
        }

        [Fact]
        public async Task SendAsync_HttpContentIsNullWhenShouldNotBe_ThrowsSiestaHttpException()
        {
            var httpRequest = new HttpRequestMessage();
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
            };
            var dto = new TestContentSiestaRequest(httpRequest);

            this.SetupMessageHandler(responseMessage, httpRequest);

            var exception = await Assert.ThrowsAsync<SiestaHttpException>(() =>
                this.sut.SendAsync(dto));

            exception.FailedHttpResponseMessage.Should().Be(responseMessage);
            exception.Message.Should().Be("Content was unexpectedly null.");
        }

        #endregion

        private void SetupMessageHandler(
            HttpResponseMessage responseMessage,
            HttpRequestMessage requestMessage)
        {
            this.httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(r => r == requestMessage),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMessage);
        }

        private void SetupMessageHandler(
            object responseContent,
            HttpRequestMessage requestMessage)
        {
            this.httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(r => r == requestMessage),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(responseContent)),
                });
        }
    }

    public class TestContentSiestaRequest : SiestaRequestBase<TestContent>
    {
        private readonly HttpRequestMessage requestMessage;

        public TestContentSiestaRequest(HttpRequestMessage? requestMessage = null) => this.requestMessage = requestMessage ?? new HttpRequestMessage();

        public Guid Id => Guid.NewGuid();

        public override HttpRequestMessage GenerateRequestMessage()
        {
            return this.requestMessage;
        }
    }

    public class TestNoContentSiestaRequest : SiestaRequestBase
    {
        private readonly HttpRequestMessage requestMessage;

        public TestNoContentSiestaRequest(HttpRequestMessage? requestMessage = null) => this.requestMessage = requestMessage ?? new HttpRequestMessage();

        public Guid Id => Guid.NewGuid();

        public override HttpRequestMessage GenerateRequestMessage()
        {
            return this.requestMessage;
        }
    }

    public class TestContent
    {
        public Guid Id { get; set; }
    }
}
