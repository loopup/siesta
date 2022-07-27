namespace Siesta.Client.Tests.Exceptions
{
    using System;
    using System.Net;
    using System.Net.Http;
    using FluentAssertions;
    using Newtonsoft.Json;
    using Siesta.Client.Exceptions;
    using Xunit;

    public class SiestaHttpCallFailedExceptionTests
    {
        #region construction

        [Fact]
        public void Construction_InnerExceptionAndHttpResponseProvided_SetsMessageExceptionAndHttpResponse()
        {
            var innerException = new Exception();
            var httpResponse = new HttpResponseMessage();

            var expectedMessage = $"HTTP call was unsuccessful. " +
                $"Response: {JsonConvert.SerializeObject(new { failedHttpResponseMessage = httpResponse, failedMessageContent = string.Empty })}";

            var siestaException = new SiestaHttpCallFailedException(innerException, httpResponse);

            siestaException.Message.Should().Be(expectedMessage);
            siestaException.InnerException.Should().Be(innerException);
            siestaException.FailedHttpResponseMessage.Should().Be(httpResponse);
        }

        [Fact]
        public void Construction_HttpResponseProvided_SetsMessageAndHttpResponseA()
        {
            var content = "Response content";
            var httpResponse = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.NotFound,
                Content = new StringContent(content),
                ReasonPhrase = "Entity not found",
            };

            var expectedMessage = $"HTTP call was unsuccessful. " +
                $"Response: {JsonConvert.SerializeObject(new { failedHttpResponseMessage = httpResponse, failedMessageContent = content })}";

            var siestaException = new SiestaHttpCallFailedException(httpResponse, content);

            siestaException.Message.Should().Be(expectedMessage);
            siestaException.FailedHttpResponseMessage.Should().Be(httpResponse);
        }

        #endregion

        #region Serialization

        [Fact]
        public void Serialization_SerializeAndDeserialize_KeepsFailedHttpResponseMessage()
        {
            var siestaException = new SiestaHttpCallFailedException(new HttpResponseMessage(HttpStatusCode.Locked));

            JsonConvert.DeserializeObject<SiestaHttpCallFailedException>(JsonConvert.SerializeObject(siestaException)).Should().BeEquivalentTo(siestaException);
        }

        #endregion
    }
}
