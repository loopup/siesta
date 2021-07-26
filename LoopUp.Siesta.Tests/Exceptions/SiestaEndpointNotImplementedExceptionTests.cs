namespace LoopUp.Siesta.Tests.Exceptions
{
    using System;
    using FluentAssertions;
    using LoopUp.Siesta.Exceptions;
    using Xunit;

    public class SiestaEndpointNotImplementedExceptionTests
    {
        #region construction

        [Fact]
        public void Construction_NoParameters_SetsMessageCorrectly()
        {
            var exception = new SiestaEndpointNotImplementedException();

            exception.Message.Should().Be("Endpoint not implemented.");
        }

        [Fact]
        public void Construction_InnerException_SetsMessageCorrectly()
        {
            var exception = new SiestaEndpointNotImplementedException(new Exception());

            exception.Message.Should().Be("Endpoint not implemented.");
        }

        [Fact]
        public void Construction_InnerException_SetsInnerExceptionCorrectly()
        {
            var inner = new Exception();
            var exception = new SiestaEndpointNotImplementedException(inner);

            exception.InnerException.Should().Be(inner);
        }

        #endregion
    }
}
