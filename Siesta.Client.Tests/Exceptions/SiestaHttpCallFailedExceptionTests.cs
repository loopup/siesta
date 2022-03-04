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

            var siestaException = new SiestaHttpCallFailedException(innerException, httpResponse);

            siestaException.Message.Should().Be("HTTP call was unsuccessful.");
            siestaException.InnerException.Should().Be(innerException);
            siestaException.FailedHttpResponseMessage.Should().Be(httpResponse);
        }

        [Fact]
        public void Construction_HttpResponseProvided_SetsMessageAndHttpResponse()
        {
            var httpResponse = new HttpResponseMessage();

            var siestaException = new SiestaHttpCallFailedException(httpResponse);

            siestaException.Message.Should().Be("HTTP call was unsuccessful.");
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
