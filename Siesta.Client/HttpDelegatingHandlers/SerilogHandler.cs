namespace Siesta.Client.HttpDelegatingHandlers
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Serilog;
    using Serilog.Context;

    /// <summary>
    /// DelegatingHandler that will add Serilog based logging to an HtpClient.
    /// </summary>
    public class SerilogHandler : DelegatingHandler
    {
        private readonly ILogger logger;
        private readonly string systemName;
        private readonly string loggerCorrelationIdName;
        private readonly string requestHeaderCorrelationIdKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerilogHandler"/> class.
        /// </summary>
        /// <param name="logger">Instance of Serilog logger.</param>
        /// <param name="systemName">Name of the current system.</param>
        /// <param name="loggerCorrelationIdName">(Optional) Key to use for correlation ID if it is present. Default is "CorrelationId".</param>
        /// <param name="requestHeaderCorrelationIdKey">(Optional) Key in request headers to look for a correlation ID. Default is "X-Correlation-ID".</param>
        public SerilogHandler(ILogger logger, string systemName, string loggerCorrelationIdName, string requestHeaderCorrelationIdKey)
        {
            this.logger = logger;
            this.loggerCorrelationIdName = loggerCorrelationIdName;
            this.requestHeaderCorrelationIdKey = requestHeaderCorrelationIdKey;
            this.systemName = systemName;
        }

        /// <inheritdoc />
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            IDisposable? disposableContext = null;
            if (request.Headers.Contains(this.requestHeaderCorrelationIdKey))
            {
                disposableContext = LogContext.PushProperty(
                    this.loggerCorrelationIdName,
                    request.Headers.GetValues(this.requestHeaderCorrelationIdKey).FirstOrDefault());
            }

            this.logger.Information(
                "Sending {SystemName} request to {Method} {Url}",
                new object[] { $"{this.systemName}:{Environment.MachineName}", request.Method, request.RequestUri! });

            var response = await base.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                #if NETSTANDARD2_0
                    this.logger.Error(
                        "Request from {SystemName} to {Method} {Url} failed with code {Code} and response body {Response}",
                        $"{this.systemName}:{Environment.MachineName}",
                        request.Method,
                        request.RequestUri,
                        response.StatusCode,
                        await response.Content.ReadAsStringAsync());
                #else
                    this.logger.Error(
                    "Request from {SystemName} to {Method} {Url} failed with code {Code} and response body {Response}",
                    $"{this.systemName}:{Environment.MachineName}",
                    request.Method,
                    request.RequestUri,
                    response.StatusCode,
                    await response.Content.ReadAsStringAsync(cancellationToken));
                #endif
            }

            disposableContext?.Dispose();

            return response;
        }
    }
}
