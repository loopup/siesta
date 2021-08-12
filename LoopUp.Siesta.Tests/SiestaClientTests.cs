namespace LoopUp.Siesta.Tests
{
    using System;
    using System.Linq;
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

            this.sut = new MyClient(httpClient);
        }

        #region SendAsync content expected

        [Fact]
        public async Task SendAsyncContentExpected_HttpCallSuccessful_ReturnsContent()
        {
            var httpRequest = new HttpRequestMessage();
            var request = new TestContentSiestaRequest(httpRequest);
            var responseContent = new TestContent
            {
                Id = request.Id,
            };

            this.SetupMessageHandler(responseContent, httpRequest);

            var result = await this.sut.SendAsync<TestContent>(request);

            result.Should().BeEquivalentTo(responseContent);
        }

        [Fact]
        public async Task SendAsyncContent_CurrentCorrelationIdProvidedCallSuccessful_ReturnsContent()
        {
            var httpRequest = new HttpRequestMessage();
            var request = new TestContentSiestaRequest(httpRequest);
            var correlationId = Guid.NewGuid().ToString();
            var responseContent = new TestContent
            {
                Id = request.Id,
            };
            var headerName = "X-Correlation-ID";
            var client = new MyClient(
                new HttpClient(this.httpMessageHandlerMock.Object)
                {
                    BaseAddress = new Uri("https://base.address.com"),
                },
                headerName);

            this.httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(r => r.Headers.GetValues(headerName).First() == correlationId),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(responseContent)),
                });

            var result = await client.SendAsync(request, correlationId);

            result.Should().BeEquivalentTo(responseContent);
        }

        [Fact]
        public void SendAsyncContent_CurrentCorrelationIdBytHeaderNotConfigured_ThrowsConfigurationException()
        {
            var clientWithoutHeaderConfigured = new MyClient(new HttpClient());

            Func<Task> action = async () => await clientWithoutHeaderConfigured.SendAsync(
                new TestContentSiestaRequest(),
                Guid.NewGuid().ToString());

            action
                .Should()
                .Throw<SiestaConfigurationException>()
                .Where(e => e.ConfigurationIssue == ConfigurationIssue.CorrelationIdHeaderNotConfigured);
        }

        [Fact]
        public async Task SendAsyncContent_HttpCallReturnsUnsuccessful_ThrowsSiestaHttpException()
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
        public async Task SendAsyncContent_HttpContentDoesNotMatchExpectedFormat_ThrowsSiestaHttpException()
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
        public async Task SendAsyncContent_HttpContentIsNullWhenShouldNotBe_ThrowsSiestaHttpException()
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
        public async Task SendAsyncNoContent_ExpectedHttpCallSuccessful_ReturnsCompletedTask()
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
        public async Task SendAsyncNoContent_CurrentCorrelationIdProvidedCallSuccessful_ReturnsCompletedTask()
        {
            var httpRequest = new HttpRequestMessage();
            var request = new TestNoContentSiestaRequest(httpRequest);
            var correlationId = Guid.NewGuid().ToString();
            var headerName = "X-Correlation-ID";
            var client = new MyClient(
                new HttpClient(this.httpMessageHandlerMock.Object)
                {
                    BaseAddress = new Uri("https://base.address.com"),
                },
                headerName);

            this.httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(r => r.Headers.GetValues(headerName).First() == correlationId),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                });

            var result = await client.SendAsync(request, correlationId);

            result.IsCompleted.Should().BeTrue();
        }

        [Fact]
        public void SendAsyncNoContent_CurrentCorrelationIdBytHeaderNotConfigured_ThrowsConfigurationException()
        {
            var clientWithoutHeaderConfigured = new MyClient(new HttpClient());

            Func<Task> action = async () => await clientWithoutHeaderConfigured.SendAsync(
                new TestNoContentSiestaRequest(),
                Guid.NewGuid().ToString());

            action
                .Should()
                .Throw<SiestaConfigurationException>()
                .Where(e => e.ConfigurationIssue == ConfigurationIssue.CorrelationIdHeaderNotConfigured);
        }

        [Fact]
        public async Task SendAsyncNoContent_ExpectedHttpCallReturnsUnsuccessful_ThrowsSiestaHttpException()
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
        public async Task SendAsyncNoContent_ExpectedHttpContentNotNull_ThrowsSiestaHttpException()
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
        public void SendAsyncPatchRequest_CurrentCorrelationIdBytHeaderNotConfigured_ThrowsConfigurationException()
        {
            var clientWithoutHeaderConfigured = new MyClient(new HttpClient());

            Func<Task> action = async () => await clientWithoutHeaderConfigured.SendAsync(
                new TestPatchRequest(new TestContent()),
                Guid.NewGuid().ToString());

            action
                .Should()
                .Throw<SiestaConfigurationException>()
                .Where(e => e.ConfigurationIssue == ConfigurationIssue.CorrelationIdHeaderNotConfigured);
        }

        [Fact]
        public async Task SendAsyncPatchRequest_FailsToGetOriginal_ThrowsSiestaHttpException()
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
        public async Task SendAsyncPatchRequest_GetHttpContentDoesNotMatchExpectedFormat_ThrowsSiestaHttpException()
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
        public async Task SendAsyncPatchRequest_GetHttpContentIsNull_ThrowsSiestaHttpException()
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
        public async Task SendAsyncPatchRequest_GetSuccessfulPatchUnsuccessful_ThrowsSiestaHttpException()
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
        public async Task SendAsyncPatchRequest_GetSuccessfulPatchContentNotAsExpected_ThrowsSiestaHttpException()
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
        public async Task SendAsyncPatchRequest_GetSuccessfulPatchContentNull_ThrowsSiestaHttpException()
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
        public async Task SendAsyncPatchRequest_CurrentCorrelationIdProvidedCallSuccessful_ReturnsResource()
        {
            var getRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
            };
            var getContent = new TestContent(Guid.NewGuid());
            var patchRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Patch,
            };
            var patchResponseContent = new TestContent(Guid.NewGuid());
            var request = new TestPatchRequest(new TestContent(Guid.NewGuid()), getRequestMessage: getRequest, requestMessage: patchRequest);
            var correlationId = Guid.NewGuid().ToString();
            var headerName = "X-Correlation-ID";
            var client = new MyClient(
                new HttpClient(this.httpMessageHandlerMock.Object)
                {
                    BaseAddress = new Uri("https://base.address.com"),
                },
                headerName);

            this.httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(r => r.Headers.GetValues(headerName).First() == correlationId && r.Method == HttpMethod.Get),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(getContent)),
                });
            this.httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(r => r.Headers.GetValues(headerName).First() == correlationId && r.Method == HttpMethod.Patch),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(patchResponseContent)),
                });

            var result = await client.SendAsync(request, correlationId);

            result.Should().BeEquivalentTo(patchResponseContent);
        }

        [Fact]
        public async Task SendAsyncPatchRequest_GetSuccessfulPatchSuccessful_ReturnsResource()
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

    internal class MyClient : SiestaClient
    {
        public MyClient(HttpClient httpClient)
            : base(httpClient)
        {
        }

        public MyClient(HttpClient httpClient, string correlationIdHeader)
            : base(httpClient, new SiestaClientConfigurationOptions { RequestHeaderCorrelationIdKey = correlationIdHeader })
        {
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
        public TestContent()
        {
        }

        public TestContent(Guid id)
        {
            this.Id = id;
        }

        public Guid Id { get; set; }
    }
}
