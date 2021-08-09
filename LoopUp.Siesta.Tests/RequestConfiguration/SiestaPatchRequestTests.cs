namespace LoopUp.Siesta.Tests.RequestConfiguration
{
    using LoopUp.Siesta.Exceptions;
    using LoopUp.Siesta.RequestConfiguration;
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
    }

    public class UnimplementedSiestaPatchRequest : SiestaPatchRequest<string>
    {
        public UnimplementedSiestaPatchRequest()
            : base("some string")
        {
        }
    }
}
