namespace LoopUp.Siesta.Tests.RequestConfiguration
{
    using LoopUp.Siesta.Exceptions;
    using LoopUp.Siesta.RequestConfiguration;
    using Xunit;

    public class SiestaRequestTests
    {
        #region GenerateRequestMessage

        [Fact]
        public void GenerateRequestMessage_NotOverridden_ThrowsRequestNotImplementedException()
        {
            var request = new UnimplementedSiestaRequest();

            Assert.Throws<SiestaRequestNotImplementedException>(() => request.GenerateRequestMessage());
        }

        #endregion
    }

    public class UnimplementedSiestaRequest : SiestaRequest
    {
    }
}
