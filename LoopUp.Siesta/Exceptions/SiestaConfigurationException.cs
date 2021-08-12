namespace LoopUp.Siesta.Exceptions
{
    using System;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;

    /// <summary>
    /// Exception thrown when the request is not implemented for a DTO.
    /// </summary>
    [Serializable]
    public class SiestaConfigurationException : Exception
    {
        private ConfigurationIssue configurationIssue;

        /// <summary>
        /// Initializes a new instance of the <see cref="SiestaConfigurationException"/> class.
        /// </summary>
        /// <param name="issue">The underlying configuration issue.</param>
        public SiestaConfigurationException(ConfigurationIssue issue)
            : base("There was an issue with the configuration of a Siesta Client. Please see the ConfigurationIssue for details.")
        {
            this.configurationIssue = issue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SiestaConfigurationException"/> class.
        /// </summary>
        /// <param name="info">Instance of <see cref="SerializationInfo"/>.</param>
        /// <param name="context">Instance of <see cref="StreamingContext"/>.</param>
        [JsonConstructor]
        protected SiestaConfigurationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            this.configurationIssue = (ConfigurationIssue)info.GetValue("ConfigurationIssue", typeof(ConfigurationIssue));
        }

        /// <summary>
        /// Gets the underlying configuration issue.
        /// </summary>
        public ConfigurationIssue ConfigurationIssue => this.configurationIssue;

        /// <inheritdoc />
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue("ConfigurationIssue", this.ConfigurationIssue);
            base.GetObjectData(info, context);
        }
    }
}
