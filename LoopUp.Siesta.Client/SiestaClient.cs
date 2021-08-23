namespace LoopUp.Siesta.Client
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using LoopUp.Siesta.Configuration.Exceptions;
    using LoopUp.Siesta.Configuration.RequestConfiguration;
    using Newtonsoft.Json;

    /// <summary>
    /// The <see cref="SiestaClient"/> that can be used to communicate with an API built using Siesta.
    /// </summary>
    public abstract class SiestaClient : ISiestaClient
    {
        private readonly HttpClient client;
        private readonly string? correlationIdHeaderName;

        /// <summary>
        /// Initializes a new instance of the <see cref="SiestaClient"/> class.
        /// </summary>
        /// <param name="httpClient">A pre-configured <see cref="HttpClient"/>.</param>
        protected SiestaClient(HttpClient httpClient)
        {
            this.client = httpClient;
            this.correlationIdHeaderName = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SiestaClient"/> class.
        /// </summary>
        /// <param name="httpClient">A pre-configured <see cref="HttpClient"/>.</param>
        /// <param name="siestaClientConfigurationOptions">Configuration options for a <see cref="SiestaClient"/>.</param>
        protected SiestaClient(HttpClient httpClient, SiestaClientConfigurationOptions siestaClientConfigurationOptions)
        {
            this.client = httpClient;
            this.correlationIdHeaderName = siestaClientConfigurationOptions.RequestHeaderCorrelationIdKey;
        }

        /// <inheritdoc />
        public async Task<Task> SendAsync(SiestaRequest siestaRequest)
        {
            return await this.SendRequestWithNoExpectedContent(siestaRequest);
        }

        /// <inheritdoc />
        public async Task<Task> SendAsync(SiestaRequest siestaRequest, string currentCorrelationId)
        {
            return await this.SendRequestWithNoExpectedContent(siestaRequest, currentCorrelationId);
        }

        /// <inheritdoc />
        public async Task<TReturn> SendAsync<TReturn>(SiestaRequest<TReturn> siestaRequest)
        {
            return await this.SendRequestWithExpectedContent<TReturn>(siestaRequest.GenerateRequestMessage());
        }

        /// <inheritdoc />
        public async Task<TReturn> SendAsync<TReturn>(SiestaRequest<TReturn> siestaRequest, string currentCorrelationId)
        {
            return await this.SendRequestWithExpectedContent<TReturn>(siestaRequest.GenerateRequestMessage(), currentCorrelationId);
        }

        /// <inheritdoc />
        public async Task<TReturn> SendAsync<TReturn>(SiestaPatchRequest<TReturn> siestaPatchRequest)
        {
            var originalResource = await this.SendRequestWithExpectedContent<TReturn>(siestaPatchRequest.GenerateGetRequestMessage());

            return await this.SendRequestWithExpectedContent<TReturn>(
                siestaPatchRequest.GeneratePatchRequestMessage(originalResource));
        }

        /// <inheritdoc />
        public async Task<TReturn> SendAsync<TReturn>(SiestaPatchRequest<TReturn> siestaPatchRequest, string currentCorrelationId)
        {
            var originalResource = await this.SendRequestWithExpectedContent<TReturn>(siestaPatchRequest.GenerateGetRequestMessage(), currentCorrelationId);

            return await this.SendRequestWithExpectedContent<TReturn>(
                siestaPatchRequest.GeneratePatchRequestMessage(originalResource), currentCorrelationId);
        }

        private async Task<Task> SendRequestWithNoExpectedContent(SiestaRequest siestaRequest, string? currentCorrelationId = null)
        {
            var requestMessage = siestaRequest.GenerateRequestMessage();

            if (currentCorrelationId is not null)
            {
                if (this.correlationIdHeaderName is null)
                {
                    throw new SiestaConfigurationException(ConfigurationIssue.CorrelationIdHeaderNotConfigured);
                }

                requestMessage.Headers.Add(this.correlationIdHeaderName, currentCorrelationId);
            }

            var response = await this.client.SendAsync(requestMessage);

            if (!response.IsSuccessStatusCode)
            {
                throw new SiestaHttpException("HTTP call was not successful.", response);
            }

            var content = await response.Content.ReadAsStringAsync();

            if (!string.IsNullOrEmpty(content))
            {
                throw new SiestaHttpException("Content was not as expected.", response);
            }

            return Task.CompletedTask;
        }

        private async Task<T> SendRequestWithExpectedContent<T>(HttpRequestMessage requestMessage, string? currentCorrelationId = null)
        {
            if (currentCorrelationId is not null)
            {
                if (this.correlationIdHeaderName is null)
                {
                    throw new SiestaConfigurationException(ConfigurationIssue.CorrelationIdHeaderNotConfigured);
                }

                requestMessage.Headers.Add(this.correlationIdHeaderName, currentCorrelationId);
            }

            var response = await this.client.SendAsync(requestMessage);

            if (!response.IsSuccessStatusCode)
            {
                throw new SiestaHttpException("HTTP call was not successful.", response);
            }

            try
            {
                var deserialized = JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());

                if (deserialized == null)
                {
                    throw new SiestaHttpException("Content was unexpectedly null.", response);
                }

                return deserialized;
            }
            catch (SiestaHttpException)
            {
                throw;
            }
            catch
            {
                throw new SiestaHttpException("Content was not as expected.", response);
            }
        }
    }
}
