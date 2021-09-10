namespace Siesta.Client.ServiceCollectionExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http.Headers;

    /// <summary>
    /// Configuration class for convenience method to setup a client with Correlation Id and Serilog.
    /// </summary>
    public class CorrelationAndLoggingConfigurationOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CorrelationAndLoggingConfigurationOptions"/> class.
        /// </summary>
        /// <param name="baseAddress">Base address for the client.</param>
        /// <param name="systemName">System name for the client.</param>
        /// <param name="defaultHeaders">(Optional) Default headers to add to client requests.</param>
        /// <param name="authenticationHeaderValue">(Optional) Authentication header value to add to all requests.</param>
        /// <param name="loggerCorrelationId">(Optional) Correlation ID key value for Serilog.</param>
        /// <param name="requestHeaderCorrelationIdKey">Correlation ID request header key.</param>
        public CorrelationAndLoggingConfigurationOptions(
            Uri baseAddress,
            string systemName,
            Dictionary<string, string>? defaultHeaders = null,
            AuthenticationHeaderValue? authenticationHeaderValue = null,
            string? loggerCorrelationId = null,
            string? requestHeaderCorrelationIdKey = null)
        {
            this.BaseAddress = baseAddress;
            this.SystemName = systemName;
            this.DefaultHeaders = defaultHeaders ?? new ();
            this.AuthenticationHeaderValue = authenticationHeaderValue;
            this.LoggerCorrelationId = loggerCorrelationId ?? "CorrelationId";
            this.RequestHeaderCorrelationIdKey = requestHeaderCorrelationIdKey ?? "X-Correlation-ID";
        }

        /// <summary>
        /// Gets or sets a value for the Base Address of the client.
        /// </summary>
        public Uri BaseAddress { get; set; }

        /// <summary>
        /// Gets or sets a value for the System name the client represents.
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        /// Gets or sets the default headers to add to each request.
        /// </summary>
        public Dictionary<string, string> DefaultHeaders { get; set; }

        /// <summary>
        /// Gets or sets the authentication header value to add to each request.
        /// </summary>
        public AuthenticationHeaderValue? AuthenticationHeaderValue { get; set; }

        /// <summary>
        /// Gets or sets the correlation id key to use for Serilog.
        /// </summary>
        public string LoggerCorrelationId { get; set; }

        /// <summary>
        /// Gets or sets the correlation id header key to use for requests.
        /// </summary>
        public string RequestHeaderCorrelationIdKey { get; set; }
    }
}
