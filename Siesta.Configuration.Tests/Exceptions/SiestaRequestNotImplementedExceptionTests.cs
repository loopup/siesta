namespace Siesta.Configuration.Tests.Exceptions
{
    using System;
    using FluentAssertions;
    using Siesta.Configuration.Exceptions;
    using Xunit;

    public class SiestaRequestNotImplementedExceptionTests
    {
        #region construction

        [Fact]
        public void Construction_NoParameters_SetsMessageCorrectly()
        {
            var exception = new SiestaRequestNotImplementedException();

            exception.Message.Should().Be("Request not implemented.");
        }

        [Fact]
        public void Construction_InnerException_SetsMessageCorrectly()
        {
            var exception = new SiestaRequestNotImplementedException(new Exception());

            exception.Message.Should().Be("Request not implemented.");
        }

        [Fact]
        public void Construction_InnerException_SetsInnerExceptionCorrectly()
        {
            var inner = new Exception();
            var exception = new SiestaRequestNotImplementedException(inner);

            exception.InnerException.Should().Be(inner);
        }

        #endregion
    }
}
