namespace LoopUp.Siesta.HttpDelegatingHandlers
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// A DelegatingHandler that will add a correlation id header to every request if one is not present.
    /// </summary>
    public class CorrelationIdHandler : DelegatingHandler
    {
        private readonly string correlationIdHeader;

        /// <summary>
        /// Initializes a new instance of the <see cref="CorrelationIdHandler"/> class.
        /// </summary>
        /// <param name="correlationIdHeader">The header to look for and or add the correlation ID to.".</param>
        public CorrelationIdHandler(string correlationIdHeader)
        {
            this.correlationIdHeader = correlationIdHeader;
        }

        /// <inheritdoc />
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!request.Headers.Contains(this.correlationIdHeader))
            {
                request.Headers.Add(this.correlationIdHeader, Guid.NewGuid().ToString());
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
