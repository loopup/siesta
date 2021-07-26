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

        #region Get

        #region Get by Guid

        [Fact]
        public async Task GetByGuid_HttpCallSuccessful_ReturnsSingle()
        {
            var expectedResult = new TestDto
            {
                Id = Guid.NewGuid(),
            };

            this.SetupMessageHandler(
                new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(expectedResult)),
                },
                r => r.RequestUri!.AbsolutePath == $"{expectedResult.GetSingleEndpoint}/{expectedResult.Id}" && r.Method == HttpMethod.Get);

            var result = await this.sut.Get<TestDto>((Guid)expectedResult.Id);

            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task GetByGuid_EndpointNull_ThrowsEndpointNotImplementedException()
        {
            await Assert.ThrowsAsync<SiestaEndpointNotImplementedException>(() =>
                this.sut.Get<EndpointsNotImplementedDto>(Guid.NewGuid()));
        }

        [Fact]
        public async Task GetByGuid_HttpCallFails_ThrowsSiestaException()
        {
            var id = Guid.NewGuid();
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Forbidden,
            };

            this.SetupMessageHandler(
                responseMessage,
                r => r.RequestUri!.AbsolutePath == $"{new TestDto().GetSingleEndpoint}/{id}" && r.Method == HttpMethod.Get);

            var exception = await Assert.ThrowsAsync<SiestaHttpException>(() =>
                this.sut.Get<TestDto>(id));

            exception.FailedHttpResponseMessage.Should().Be(responseMessage);
            exception.Message.Should().Be("HTTP call was not successful.");
        }

        [Fact]
        public async Task GetByGuid_HttpContentDoesNotMatchExpectedFormat_ThrowsSiestaException()
        {
            var id = Guid.NewGuid();
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("just a string"),
            };

            this.SetupMessageHandler(
                responseMessage,
                r => r.RequestUri!.AbsolutePath == $"{new TestDto().GetSingleEndpoint}/{id}" && r.Method == HttpMethod.Get);

            var exception = await Assert.ThrowsAsync<SiestaHttpException>(() =>
                this.sut.Get<TestDto>(id));

            exception.FailedHttpResponseMessage.Should().Be(responseMessage);
            exception.Message.Should().Be("Content was not as expected.");
        }

        [Fact]
        public async Task GetByGuid_HttpContentIsNull_ThrowsSiestaException()
        {
            var id = Guid.NewGuid();
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
            };

            this.SetupMessageHandler(
                responseMessage,
                r => r.RequestUri!.AbsolutePath == $"{new TestDto().GetSingleEndpoint}/{id}" && r.Method == HttpMethod.Get);

            var exception = await Assert.ThrowsAsync<SiestaHttpException>(() =>
                this.sut.Get<TestDto>(id));

            exception.FailedHttpResponseMessage.Should().Be(responseMessage);
            exception.Message.Should().Be("Content was not as expected.");
        }

        #endregion

        #region Get by string

        [Fact]
        public async Task GetByString_HttpCallSuccessful_ReturnsSingle()
        {
            var expectedResult = new TestDto
            {
                StringId = "stringId",
            };

            this.SetupMessageHandler(
                new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(expectedResult)),
                },
                r => r.RequestUri!.AbsolutePath == $"{expectedResult.GetSingleEndpoint}/{expectedResult.StringId}" && r.Method == HttpMethod.Get);

            var result = await this.sut.Get<TestDto>(expectedResult.StringId);

            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task GetByString_EndpointNull_ThrowsEndpointNotImplementedException()
        {
            await Assert.ThrowsAsync<SiestaEndpointNotImplementedException>(() =>
                this.sut.Get<EndpointsNotImplementedDto>("anything"));
        }

        [Fact]
        public async Task GetByString_HttpCallFails_ThrowsSiestaException()
        {
            var id = "stringId";
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Forbidden,
            };

            this.SetupMessageHandler(
                responseMessage,
                r => r.RequestUri!.AbsolutePath == $"{new TestDto().GetSingleEndpoint}/{id}" && r.Method == HttpMethod.Get);

            var exception = await Assert.ThrowsAsync<SiestaHttpException>(() =>
                this.sut.Get<TestDto>(id));

            exception.FailedHttpResponseMessage.Should().Be(responseMessage);
            exception.Message.Should().Be("HTTP call was not successful.");
        }

        [Fact]
        public async Task GetByString_HttpContentDoesNotMatchExpectedFormat_ThrowsSiestaException()
        {
            var id = "stringId";
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("just a string"),
            };

            this.SetupMessageHandler(
                responseMessage,
                r => r.RequestUri!.AbsolutePath == $"{new TestDto().GetSingleEndpoint}/{id}" && r.Method == HttpMethod.Get);

            var exception = await Assert.ThrowsAsync<SiestaHttpException>(() =>
                this.sut.Get<TestDto>(id));

            exception.FailedHttpResponseMessage.Should().Be(responseMessage);
            exception.Message.Should().Be("Content was not as expected.");
        }

        [Fact]
        public async Task GetByString_HttpContentIsNull_ThrowsSiestaException()
        {
            var id = "stringId";
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
            };

            this.SetupMessageHandler(
                responseMessage,
                r => r.RequestUri!.AbsolutePath == $"{new TestDto().GetSingleEndpoint}/{id}" && r.Method == HttpMethod.Get);

            var exception = await Assert.ThrowsAsync<SiestaHttpException>(() =>
                this.sut.Get<TestDto>(id));

            exception.FailedHttpResponseMessage.Should().Be(responseMessage);
            exception.Message.Should().Be("Content was not as expected.");
        }

        #endregion

        #region Get by int

        [Fact]
        public async Task GetByInt_HttpCallSuccessful_ReturnsSingle()
        {
            var expectedResult = new TestDto
            {
                IntId = 11,
            };

            this.SetupMessageHandler(
                new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(expectedResult)),
                },
                r => r.RequestUri!.AbsolutePath == $"{expectedResult.GetSingleEndpoint}/{expectedResult.IntId}" && r.Method == HttpMethod.Get);

            var result = await this.sut.Get<TestDto>((int)expectedResult.IntId);

            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task GetByInt_EndpointNull_ThrowsEndpointNotImplementedException()
        {
            await Assert.ThrowsAsync<SiestaEndpointNotImplementedException>(() =>
                this.sut.Get<EndpointsNotImplementedDto>(10));
        }

        [Fact]
        public async Task GetByInt_HttpCallFails_ThrowsSiestaException()
        {
            var id = 11;
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Forbidden,
            };

            this.SetupMessageHandler(
                responseMessage,
                r => r.RequestUri!.AbsolutePath == $"{new TestDto().GetSingleEndpoint}/{id}" && r.Method == HttpMethod.Get);

            var exception = await Assert.ThrowsAsync<SiestaHttpException>(() =>
                this.sut.Get<TestDto>(id));

            exception.FailedHttpResponseMessage.Should().Be(responseMessage);
            exception.Message.Should().Be("HTTP call was not successful.");
        }

        [Fact]
        public async Task GetByInt_HttpContentDoesNotMatchExpectedFormat_ThrowsSiestaException()
        {
            var id = 11;
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("just a string"),
            };

            this.SetupMessageHandler(
                responseMessage,
                r => r.RequestUri!.AbsolutePath == $"{new TestDto().GetSingleEndpoint}/{id}" && r.Method == HttpMethod.Get);

            var exception = await Assert.ThrowsAsync<SiestaHttpException>(() =>
                this.sut.Get<TestDto>(id));

            exception.FailedHttpResponseMessage.Should().Be(responseMessage);
            exception.Message.Should().Be("Content was not as expected.");
        }

        [Fact]
        public async Task GetByInt_HttpContentIsNull_ThrowsSiestaException()
        {
            var id = 11;
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
            };

            this.SetupMessageHandler(
                responseMessage,
                r => r.RequestUri!.AbsolutePath == $"{new TestDto().GetSingleEndpoint}/{id}" && r.Method == HttpMethod.Get);

            var exception = await Assert.ThrowsAsync<SiestaHttpException>(() =>
                this.sut.Get<TestDto>(id));

            exception.FailedHttpResponseMessage.Should().Be(responseMessage);
            exception.Message.Should().Be("Content was not as expected.");
        }

        #endregion

        #region Get many

        [Fact]
        public async Task GetMany_HttpCallSuccessful_ReturnsPaginatedResults()
        {
            var testDtos = new List<TestDto>
            {
                new TestDto() { Id = Guid.NewGuid() },
                new TestDto() { Id = Guid.NewGuid() },
            };

            var expectedResult = new DeserializedPagedList<TestDto>(testDtos, 1, 10, 2);
            var responseContent = new SerializablePagedList<TestDto>(testDtos, 1, 10);
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(responseContent)),
            };

            this.SetupMessageHandler(
                responseMessage,
                r => r.RequestUri!.AbsolutePath.ToString() == new TestDto().GetManyEndpoint && r.Method == HttpMethod.Get);

            var result = await this.sut.Get<TestDto>();

            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task GetMany_EndpointNull_ThrowsEndpointNotImplementedException()
        {
            await Assert.ThrowsAsync<SiestaEndpointNotImplementedException>(() =>
                this.sut.Get<EndpointsNotImplementedDto>());
        }

        [Fact]
        public async Task GetMany_HttpCallFails_ThrowsSiestaException()
        {
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError,
            };

            this.SetupMessageHandler(
                responseMessage,
                r => r.RequestUri!.AbsolutePath == new TestDto().GetManyEndpoint && r.Method == HttpMethod.Get);

            var exception = await Assert.ThrowsAsync<SiestaHttpException>(() =>
                this.sut.Get<TestDto>());

            exception.FailedHttpResponseMessage.Should().Be(responseMessage);
            exception.Message.Should().Be("HTTP call was not successful.");
        }

        [Fact]
        public async Task GetMany_HttpContentDoesNotMatchExpectedFormat_ThrowsSiestaException()
        {
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("just a string"),
            };

            this.SetupMessageHandler(
                responseMessage,
                r => r.RequestUri!.AbsolutePath == new TestDto().GetSingleEndpoint && r.Method == HttpMethod.Get);

            var exception = await Assert.ThrowsAsync<SiestaHttpException>(() =>
                this.sut.Get<TestDto>());

            exception.FailedHttpResponseMessage.Should().Be(responseMessage);
            exception.Message.Should().Be("Content was not as expected.");
        }

        [Fact]
        public async Task GetMany_HttpContentIsNull_ThrowsSiestaException()
        {
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
            };

            this.SetupMessageHandler(
                responseMessage,
                r => r.RequestUri!.AbsolutePath == new TestDto().GetSingleEndpoint && r.Method == HttpMethod.Get);

            var exception = await Assert.ThrowsAsync<SiestaHttpException>(() =>
                this.sut.Get<TestDto>());

            exception.FailedHttpResponseMessage.Should().Be(responseMessage);
            exception.Message.Should().Be("Content was not as expected.");
        }

        #endregion

        #region Get many with filter

        [Fact]
        public async Task GetManyWithFilter_HttpCallSuccessful_ReturnsPaginatedData()
        {
            var testFilters = new TestEnumerableFilterInformation
            {
                Name = "Some name",
            };
            var testDtos = new List<TestDto>
            {
                new TestDto() { Id = Guid.NewGuid() },
                new TestDto() { Id = Guid.NewGuid() },
            };

            var expectedResult = new DeserializedPagedList<TestDto>(testDtos, 1, 10, 2);
            var responseContent = new SerializablePagedList<TestDto>(testDtos, 1, 10);
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(responseContent)),
            };

            this.SetupMessageHandler(
                responseMessage,
                r => r.Method == HttpMethod.Get &&
                     r.RequestUri!.PathAndQuery == QueryHelpers.AddQueryString(new TestDto().GetManyEndpoint, testFilters.AsQueryDictionary()));

            var result = await this.sut.Get<TestDto>(testFilters);

            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task GetManyWithFilter_EndpointNull_ThrowsEndpointNotImplementedException()
        {
            await Assert.ThrowsAsync<SiestaEndpointNotImplementedException>(() =>
                this.sut.Get<EndpointsNotImplementedDto>(new EnumerableFilterInformation()));
        }

        [Fact]
        public async Task GetManyWithFilter_HttpCallFails_ThrowsSiestaException()
        {
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError,
            };
            var testFilters = new TestEnumerableFilterInformation();

            this.SetupMessageHandler(
                responseMessage,
                r => r.Method == HttpMethod.Get &&
                     r.RequestUri!.PathAndQuery == QueryHelpers.AddQueryString(new TestDto().GetManyEndpoint, testFilters.AsQueryDictionary()));

            var exception = await Assert.ThrowsAsync<SiestaHttpException>(() =>
                this.sut.Get<TestDto>(testFilters));

            exception.FailedHttpResponseMessage.Should().Be(responseMessage);
            exception.Message.Should().Be("HTTP call was not successful.");
        }

        [Fact]
        public async Task GetManyWithFilter_HttpContentDoesNotMatchExpectedFormat_ThrowsSiestaException()
        {
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("just a string"),
            };
            var testFilters = new TestEnumerableFilterInformation();

            this.SetupMessageHandler(
                responseMessage,
                r => r.Method == HttpMethod.Get &&
                     r.RequestUri!.PathAndQuery == QueryHelpers.AddQueryString(new TestDto().GetManyEndpoint, testFilters.AsQueryDictionary()));

            var exception = await Assert.ThrowsAsync<SiestaHttpException>(() =>
                this.sut.Get<TestDto>(testFilters));

            exception.FailedHttpResponseMessage.Should().Be(responseMessage);
            exception.Message.Should().Be("Content was not as expected.");
        }

        [Fact]
        public async Task GetManyWithFilter_HttpContentIsNull_ThrowsSiestaException()
        {
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
            };
            var testFilters = new TestEnumerableFilterInformation();

            this.SetupMessageHandler(
                responseMessage,
                r => r.Method == HttpMethod.Get &&
                     r.RequestUri!.PathAndQuery == QueryHelpers.AddQueryString(new TestDto().GetManyEndpoint, testFilters.AsQueryDictionary()));

            var exception = await Assert.ThrowsAsync<SiestaHttpException>(() =>
                this.sut.Get<TestDto>(testFilters));

            exception.FailedHttpResponseMessage.Should().Be(responseMessage);
            exception.Message.Should().Be("Content was not as expected.");
        }

        #endregion

        #endregion

        private void SetupMessageHandler(
            HttpResponseMessage responseMessage,
            Expression<Func<HttpRequestMessage, bool>>? requestCriteria = null)
        {
            this.httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(requestCriteria),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMessage);
        }
    }

    public class EndpointsNotImplementedDto : DtoBase
    {
    }
}
