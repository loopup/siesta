namespace LoopUp.Siesta.Configuration.Exceptions
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Enumeration of possible Siesta configuration issues.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ConfigurationIssue
    {
        /// <summary>
        /// An attempt was made to use a Correlation Id, but this has not been configured in the client.
        /// </summary>
        CorrelationIdHeaderNotConfigured,
    }
}
