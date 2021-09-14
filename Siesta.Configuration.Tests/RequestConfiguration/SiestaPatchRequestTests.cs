namespace Siesta.Configuration.Tests.RequestConfiguration
{
    using System;
    using Siesta.Configuration.Exceptions;
    using Siesta.Configuration.RequestConfiguration;
    using Xunit;

    public class SiestaPatchRequestTests
    {
        #region GeneratePatchRequestMessage

        [Fact]
        public void GeneratePatchRequestMessage_NotOverridden_ThrowsRequestNotImplementedException()
        {
            var request = new UnimplementedSiestaPatchRequest();

            Assert.Throws<SiestaRequestNotImplementedException>(() => request.GeneratePatchRequestMessage("string"));
        }

        #endregion

        #region GenerateGetRequestMessage

        [Fact]
        public void GenerateGetRequestMessage_NotOverridden_ThrowsRequestNotImplementedException()
        {
            var request = new UnimplementedSiestaPatchRequest();

            Assert.Throws<SiestaRequestNotImplementedException>(() => request.GenerateGetRequestMessage());
        }

        #endregion

        #region ExtractResourceFromGetReturn

        [Fact]
        public void ExtractResourceFromGetReturn_NotOverridden_ThrowsRequestNotImplementedException()
        {
            var request = new UnimplementedSiestaPatchRequest();

            Assert.Throws<SiestaRequestNotImplementedException>(() => request.ExtractResourceFromGetReturn(10));
        }

        #endregion

        #region ExtractResourceFromReturn

        [Fact]
        public void ExtractResourceFromReturn_NotOverridden_ThrowsRequestNotImplementedException()
        {
            var request = new UnimplementedSiestaPatchRequest();

            Assert.Throws<SiestaRequestNotImplementedException>(() => request.ExtractResourceFromReturn(Guid.Empty));
        }

        #endregion
    }

    public class UnimplementedSiestaPatchRequest : SiestaPatchRequest<Guid, string, int>
    {
        public UnimplementedSiestaPatchRequest()
            : base("some string")
        {
        }
    }
}
