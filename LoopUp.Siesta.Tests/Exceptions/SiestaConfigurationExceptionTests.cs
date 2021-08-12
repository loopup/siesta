namespace LoopUp.Siesta.Tests.Exceptions
{
    using FluentAssertions;
    using LoopUp.Siesta.Exceptions;
    using Newtonsoft.Json;
    using Xunit;

    public class SiestaConfigurationExceptionTests
    {
        #region Construction

        [Fact]
        public void Construction_Always_SetsMessageAndIssueCorrectly()
        {
            var exception = new SiestaConfigurationException(ConfigurationIssue.CorrelationIdHeaderNotConfigured);

            exception.Message
                .Should()
                .Be("There was an issue with the configuration of a Siesta Client. Please see the ConfigurationIssue for details.");
            exception.ConfigurationIssue.Should().Be(ConfigurationIssue.CorrelationIdHeaderNotConfigured);
        }

        #endregion

        #region Serialization

        [Fact]
        public void Serialization_SerializeAndDeserialize_KeepsConfigurationIssue()
        {
            var exception = new SiestaConfigurationException(ConfigurationIssue.CorrelationIdHeaderNotConfigured);

            JsonConvert.DeserializeObject<SiestaConfigurationException>(JsonConvert.SerializeObject(exception)) !
                .ConfigurationIssue.Should().BeEquivalentTo(ConfigurationIssue.CorrelationIdHeaderNotConfigured);
        }

        #endregion
    }
}
