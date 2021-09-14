namespace Siesta.Configuration.Tests.RequestConfiguration
{
    using FluentAssertions;
    using Siesta.Configuration.Exceptions;
    using Siesta.Configuration.RequestConfiguration;
    using Xunit;

    public class SiestaRequestTests
    {
        #region GenerateRequestMessage

        [Fact]
        public void GenerateRequestMessage_NotOverridden_ThrowsRequestNotImplementedException()
        {
            var unimplementedSiestaRequestNoContent = new UnimplementedSiestaRequestNoContent();
            var unimplementedSiestaRequestContent = new UnimplementedSiestaRequestContent();
            var unimplementedSiestaRequestContentWithReturnType = new UnimplementedSiestaRequestContentWithReturnType();

            Assert.Throws<SiestaRequestNotImplementedException>(() => unimplementedSiestaRequestNoContent.GenerateRequestMessage());
            Assert.Throws<SiestaRequestNotImplementedException>(() => unimplementedSiestaRequestContent.GenerateRequestMessage());
            Assert.Throws<SiestaRequestNotImplementedException>(() => unimplementedSiestaRequestContentWithReturnType.GenerateRequestMessage());
        }

        #endregion

        #region ExtractResourceFromReturn

        [Fact]
        public void ExtractResourceFromReturnWithReturnType_NotOverridden_ThrowsRequestNotImplementedException()
        {
            var unimplementedSiestaRequestContentWithReturnType = new UnimplementedSiestaRequestContentWithReturnType();

            Assert.Throws<SiestaRequestNotImplementedException>(() => unimplementedSiestaRequestContentWithReturnType.ExtractResourceFromReturn(10));
        }

        [Fact]
        public void ExtractResourceFromReturnWithNoReturnType_NotOverridden_ReturnsParameter()
        {
            var unimplementedSiestaRequestContent = new UnimplementedSiestaRequestContent();

            unimplementedSiestaRequestContent.ExtractResourceFromReturn("String").Should().Be("String");
        }

        #endregion
    }

    public class UnimplementedSiestaRequestNoContent : SiestaRequest
    {
    }

    public class UnimplementedSiestaRequestContent : SiestaRequest<string>
    {
    }

    public class UnimplementedSiestaRequestContentWithReturnType : SiestaRequest<string, int>
    {
    }
}
