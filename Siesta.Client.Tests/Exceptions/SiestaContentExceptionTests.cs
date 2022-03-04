namespace Siesta.Client.Tests.Exceptions
{
    using System;
    using System.Net;
    using System.Net.Http;
    using FluentAssertions;
    using Newtonsoft.Json;
    using Siesta.Client.Exceptions;
    using Xunit;

    public class SiestaContentExceptionTests
    {
        #region construction

        [Fact]
        public void Construction_InnerExceptionAndHttpResponseProvided_SetsMessageExceptionAndHttpResponse()
        {
            var innerException = new Exception();
            var httpResponse = new HttpResponseMessage();

            var siestaException = new SiestaContentException(innerException, httpResponse);

            siestaException.Message.Should().Be("HTTP response content was not as expected.");
            siestaException.InnerException.Should().Be(innerException);
            siestaException.HttpResponseMessage.Should().Be(httpResponse);
        }

        [Fact]
        public void Construction_HttpResponseProvided_SetsMessageAndHttpResponse()
        {
            var httpResponse = new HttpResponseMessage();

            var siestaException = new SiestaContentException(httpResponse);

            siestaException.Message.Should().Be("HTTP response content was not as expected.");
            siestaException.HttpResponseMessage.Should().Be(httpResponse);
        }

        #endregion

        #region Serialization

        [Fact]
        public void Serialization_SerializeAndDeserialize_KeepsFailedHttpResponseMessage()
        {
            var siestaException = new SiestaContentException(new HttpResponseMessage(HttpStatusCode.Locked));

            JsonConvert.DeserializeObject<SiestaContentException>(JsonConvert.SerializeObject(siestaException)).Should().BeEquivalentTo(siestaException);
        }

        #endregion
    }
}
