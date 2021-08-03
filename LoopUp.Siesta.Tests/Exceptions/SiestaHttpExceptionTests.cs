namespace LoopUp.Siesta.Tests.Exceptions
{
    using System;
    using System.Net;
    using System.Net.Http;
    using FluentAssertions;
    using LoopUp.Siesta.Exceptions;
    using Newtonsoft.Json;
    using Xunit;

    public class SiestaHttpExceptionTests
    {
        #region construction

        [Fact]
        public void Construction_MessageInnerExceptionAndHttpResponseProvided_SetsMessageExceptionAndHttpResponse()
        {
            var message = "This is the message.";
            var innerException = new Exception();
            var httpResponse = new HttpResponseMessage();

            var siestaException = new SiestaHttpException(message, innerException, httpResponse);

            siestaException.Message.Should().Be(message);
            siestaException.InnerException.Should().Be(innerException);
            siestaException.FailedHttpResponseMessage.Should().Be(httpResponse);
        }

        [Fact]
        public void Construction_MessageAndHttpResponseProvided_SetsMessageAndHttpResponse()
        {
            var message = "This is the message.";
            var httpResponse = new HttpResponseMessage();

            var siestaException = new SiestaHttpException(message, httpResponse);

            siestaException.Message.Should().Be(message);
            siestaException.FailedHttpResponseMessage.Should().Be(httpResponse);
        }

        #endregion

        #region Serialization

        [Fact]
        public void Serialization_SerializeAndDeserialize_KeepsFailedHttpResponseMessage()
        {
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.Locked);

            var siestaException = new SiestaHttpException("HTTP call failed", httpResponseMessage);

            JsonConvert.DeserializeObject<SiestaHttpException>(JsonConvert.SerializeObject(siestaException)) !
                .FailedHttpResponseMessage.Should().BeEquivalentTo(httpResponseMessage);
        }

        #endregion
    }
}
