namespace LoopUp.Siesta.Tests
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using LoopUp.Siesta.Exceptions;
    using LoopUp.Siesta.RequestConfiguration;
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

        #region SendAsync content expected

        [Fact]
        public async Task SendAsync_HttpCallSuccessful_ReturnsContent()
        {
            var httpRequest = new HttpRequestMessage();
            var request = new TestContentSiestaRequest(httpRequest);
            var responseContent = new TestContent
            {
                Id = request.Id,
            };

            this.SetupMessageHandler(responseContent, httpRequest);

            var result = await this.sut.SendAsync(request);

            result.Should().BeEquivalentTo(responseContent);
        }

        [Fact]
        public async Task SendAsync_HttpCallReturnsUnsuccessful_ThrowsSiestaHttpException()
        {
            var httpRequest = new HttpRequestMessage();
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
            };
            var request = new TestContentSiestaRequest(httpRequest);

            this.SetupMessageHandler(responseMessage, httpRequest);

            var exception = await Assert.ThrowsAsync<SiestaHttpException>(() =>
                this.sut.SendAsync(request));

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
            var request = new TestContentSiestaRequest(httpRequest);

            this.SetupMessageHandler(responseMessage, httpRequest);

            var exception = await Assert.ThrowsAsync<SiestaHttpException>(() =>
                this.sut.SendAsync(request));

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
            var request = new TestContentSiestaRequest(httpRequest);

            this.SetupMessageHandler(responseMessage, httpRequest);

            var exception = await Assert.ThrowsAsync<SiestaHttpException>(() =>
                this.sut.SendAsync(request));

            exception.FailedHttpResponseMessage.Should().Be(responseMessage);
            exception.Message.Should().Be("Content was unexpectedly null.");
        }

        #endregion

        #region SendAsync no content expected

        [Fact]
        public async Task SendAsync_NoContentExpectedHttpCallSuccessful_ReturnsCompletedTask()
        {
            var httpRequest = new HttpRequestMessage();
            var request = new TestNoContentSiestaRequest(httpRequest);
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
            };

            this.SetupMessageHandler(responseMessage, httpRequest);

            var result = await this.sut.SendAsync(request);

            result.IsCompleted.Should().BeTrue();
        }

        [Fact]
        public async Task SendAsync_NoContentExpectedHttpCallReturnsUnsuccessful_ThrowsSiestaHttpException()
        {
            var httpRequest = new HttpRequestMessage();
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
            };
            var request = new TestNoContentSiestaRequest(httpRequest);

            this.SetupMessageHandler(responseMessage, httpRequest);

            var exception = await Assert.ThrowsAsync<SiestaHttpException>(() =>
                this.sut.SendAsync(request));

            exception.FailedHttpResponseMessage.Should().Be(responseMessage);
            exception.Message.Should().Be("HTTP call was not successful.");
        }

        [Fact]
        public async Task SendAsync_NoContentExpectedHttpContentNotNull_ThrowsSiestaHttpException()
        {
            var httpRequest = new HttpRequestMessage();
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("just a string"),
            };
            var request = new TestNoContentSiestaRequest(httpRequest);

            this.SetupMessageHandler(responseMessage, httpRequest);

            var exception = await Assert.ThrowsAsync<SiestaHttpException>(() =>
                this.sut.SendAsync(request));

            exception.FailedHttpResponseMessage.Should().Be(responseMessage);
            exception.Message.Should().Be("Content was not as expected.");
        }

        #endregion

        #region SendAsync patch request

        [Fact]
        public async Task SendAsync_PatchRequestFailsToGetOriginal_ThrowsSiestaHttpException()
        {
            var getRequest = new HttpRequestMessage();
            var getResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
            };
            var request = new TestPatchRequest(new TestContent(), getRequestMessage: getRequest);

            this.SetupMessageHandler(getResponse, getRequest);

            var exception = await Assert.ThrowsAsync<SiestaHttpException>(() =>
                this.sut.SendAsync(request));

            exception.FailedHttpResponseMessage.Should().Be(getResponse);
            exception.Message.Should().Be("HTTP call was not successful.");
        }

        [Fact]
        public async Task SendAsync_PatchRequestGetHttpContentDoesNotMatchExpectedFormat_ThrowsSiestaHttpException()
        {
            var getRequest = new HttpRequestMessage();
            var getResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("just a string"),
            };
            var request = new TestPatchRequest(new TestContent(), getRequestMessage: getRequest);

            this.SetupMessageHandler(getResponse, getRequest);

            var exception = await Assert.ThrowsAsync<SiestaHttpException>(() =>
                this.sut.SendAsync(request));

            exception.FailedHttpResponseMessage.Should().Be(getResponse);
            exception.Message.Should().Be("Content was not as expected.");
        }

        [Fact]
        public async Task SendAsync_PatchRequestGetHttpContentIsNull_ThrowsSiestaHttpException()
        {
            var getRequest = new HttpRequestMessage();
            var getResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = null,
            };
            var request = new TestPatchRequest(new TestContent(), getRequestMessage: getRequest);

            this.SetupMessageHandler(getResponse, getRequest);

            var exception = await Assert.ThrowsAsync<SiestaHttpException>(() =>
                this.sut.SendAsync(request));

            exception.FailedHttpResponseMessage.Should().Be(getResponse);
            exception.Message.Should().Be("Content was unexpectedly null.");
        }

        [Fact]
        public async Task SendAsync_PatchRequestGetSuccessfulPatchUnsuccessful_ThrowsSiestaHttpException()
        {
            var getRequest = new HttpRequestMessage();
            var getContent = new TestContent();
            var patchRequest = new HttpRequestMessage();
            var patchResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
            };
            var request = new TestPatchRequest(new TestContent(), getRequestMessage: getRequest, requestMessage: patchRequest);

            this.SetupMessageHandler(getContent, getRequest);
            this.SetupMessageHandler(patchResponse, patchRequest);

            var exception = await Assert.ThrowsAsync<SiestaHttpException>(() =>
                this.sut.SendAsync(request));

            exception.FailedHttpResponseMessage.Should().Be(patchResponse);
            exception.Message.Should().Be("HTTP call was not successful.");
        }

        [Fact]
        public async Task SendAsync_PatchRequestGetSuccessfulPatchContentNotAsExpected_ThrowsSiestaHttpException()
        {
            var getRequest = new HttpRequestMessage();
            var getContent = new TestContent();
            var patchRequest = new HttpRequestMessage();
            var patchResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("just a string"),
            };
            var request = new TestPatchRequest(new TestContent(), getRequestMessage: getRequest, requestMessage: patchRequest);

            this.SetupMessageHandler(getContent, getRequest);
            this.SetupMessageHandler(patchResponse, patchRequest);

            var exception = await Assert.ThrowsAsync<SiestaHttpException>(() =>
                this.sut.SendAsync(request));

            exception.FailedHttpResponseMessage.Should().Be(patchResponse);
            exception.Message.Should().Be("Content was not as expected.");
        }

        [Fact]
        public async Task SendAsync_PatchRequestGetSuccessfulPatchContentNull_ThrowsSiestaHttpException()
        {
            var getRequest = new HttpRequestMessage();
            var getContent = new TestContent();
            var patchRequest = new HttpRequestMessage();
            var patchResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = null,
            };
            var request = new TestPatchRequest(new TestContent(), getRequestMessage: getRequest, requestMessage: patchRequest);

            this.SetupMessageHandler(getContent, getRequest);
            this.SetupMessageHandler(patchResponse, patchRequest);

            var exception = await Assert.ThrowsAsync<SiestaHttpException>(() =>
                this.sut.SendAsync(request));

            exception.FailedHttpResponseMessage.Should().Be(patchResponse);
            exception.Message.Should().Be("Content was unexpectedly null.");
        }

        [Fact]
        public async Task SendAsync_PatchRequestGetSuccessfulPatchSuccessful_ReturnsResource()
        {
            var getRequest = new HttpRequestMessage();
            var getContent = new TestContent();
            var patchRequest = new HttpRequestMessage();
            var patchResponseContent = new TestContent();
            var request = new TestPatchRequest(new TestContent(), getRequestMessage: getRequest, requestMessage: patchRequest);

            this.SetupMessageHandler(getContent, getRequest);
            this.SetupMessageHandler(patchResponseContent, patchRequest);

            var result = await this.sut.SendAsync(request);

            result.Should().BeEquivalentTo(patchResponseContent);
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

    internal class TestContentSiestaRequest : SiestaRequest<TestContent>
    {
        private readonly HttpRequestMessage requestMessage;

        public TestContentSiestaRequest(HttpRequestMessage? requestMessage = null) => this.requestMessage = requestMessage ?? new HttpRequestMessage();

        public Guid Id => Guid.NewGuid();

        public override HttpRequestMessage GenerateRequestMessage()
        {
            return this.requestMessage;
        }
    }

    internal class TestNoContentSiestaRequest : SiestaRequest
    {
        private readonly HttpRequestMessage requestMessage;

        public TestNoContentSiestaRequest(HttpRequestMessage? requestMessage = null) => this.requestMessage = requestMessage ?? new HttpRequestMessage();

        public Guid Id => Guid.NewGuid();

        public override HttpRequestMessage GenerateRequestMessage()
        {
            return this.requestMessage;
        }
    }

    internal class TestPatchRequest : SiestaPatchRequest<TestContent>
    {
        private readonly HttpRequestMessage requestMessage;
        private readonly HttpRequestMessage getRequestMessage;

        public TestPatchRequest(TestContent testContent, HttpRequestMessage? requestMessage = null, HttpRequestMessage? getRequestMessage = null)
            : base(testContent)
        {
            this.requestMessage = requestMessage ?? new HttpRequestMessage();
            this.getRequestMessage = getRequestMessage ?? new HttpRequestMessage();
        }

        public override HttpRequestMessage GeneratePatchRequestMessage(TestContent originalResource)
        {
            return this.requestMessage;
        }

        public override HttpRequestMessage GenerateGetRequestMessage()
        {
            return this.getRequestMessage;
        }
    }

    internal class TestContent
    {
        public Guid Id { get; set; }
    }
}
