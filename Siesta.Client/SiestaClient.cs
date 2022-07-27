namespace Siesta.Client
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Siesta.Client.Exceptions;
    using Siesta.Configuration.RequestConfiguration;

    /// <summary>
    /// The <see cref="SiestaClient"/> that can be used to communicate with an API built using Siesta.
    /// </summary>
    public class SiestaClient : ISiestaClient
    {
        private readonly HttpClient client;
        private readonly string? correlationIdHeaderName;

        /// <summary>
        /// Initializes a new instance of the <see cref="SiestaClient"/> class.
        /// </summary>
        /// <param name="httpClient">A pre-configured <see cref="HttpClient"/>.</param>
        public SiestaClient(HttpClient httpClient)
        {
            this.client = httpClient;
            this.correlationIdHeaderName = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SiestaClient"/> class.
        /// </summary>
        /// <param name="httpClient">A pre-configured <see cref="HttpClient"/>.</param>
        /// <param name="siestaClientConfigurationOptions">Configuration options for a <see cref="SiestaClient"/>.</param>
        public SiestaClient(HttpClient httpClient, SiestaClientConfigurationOptions siestaClientConfigurationOptions)
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
        public async Task<Task> SendAsync(SiestaRequest siestaRequest, string? currentCorrelationId)
        {
            return await this.SendRequestWithNoExpectedContent(siestaRequest, currentCorrelationId);
        }

        /// <inheritdoc />
        public async Task<TReturn> SendAsync<TResource, TReturn>(SiestaRequest<TResource, TReturn> siestaRequest)
        {
            return await this.SendRequestWithExpectedContent<TReturn>(siestaRequest.GenerateRequestMessage());
        }

        /// <inheritdoc />
        public async Task<TReturn> SendAsync<TResource, TReturn>(SiestaRequest<TResource, TReturn> siestaRequest, string? currentCorrelationId)
        {
            return await this.SendRequestWithExpectedContent<TReturn>(siestaRequest.GenerateRequestMessage(), currentCorrelationId);
        }

        /// <inheritdoc />
        public async Task<TReturn> SendAsync<TReturn, TResource, TGetReturn>(
            SiestaPatchRequest<TReturn, TResource, TGetReturn> siestaPatchRequest)
        {
            var getReturnObject = await this.SendRequestWithExpectedContent<TGetReturn>(siestaPatchRequest.GenerateGetRequestMessage());

            var originalResource = siestaPatchRequest.ExtractResourceFromGetReturn(getReturnObject);

            return await this.SendRequestWithExpectedContent<TReturn>(
                siestaPatchRequest.GeneratePatchRequestMessage(originalResource));
        }

        /// <inheritdoc />
        public async Task<TReturn> SendAsync<TReturn, TResource, TGetReturn>(
            SiestaPatchRequest<TReturn, TResource, TGetReturn> siestaPatchRequest,
            string? currentCorrelationId)
        {
            var getReturnObject = await this.SendRequestWithExpectedContent<TGetReturn>(siestaPatchRequest.GenerateGetRequestMessage(), currentCorrelationId);

            var originalResource = siestaPatchRequest.ExtractResourceFromGetReturn(getReturnObject);

            return await this.SendRequestWithExpectedContent<TReturn>(
                siestaPatchRequest.GeneratePatchRequestMessage(originalResource), currentCorrelationId);
        }

        private async Task<Task> SendRequestWithNoExpectedContent(SiestaRequest siestaRequest, string? currentCorrelationId = null)
        {
            var requestMessage = siestaRequest.GenerateRequestMessage();

            if (currentCorrelationId is not null && this.correlationIdHeaderName is not null)
            {
                requestMessage.Headers.Add(this.correlationIdHeaderName, currentCorrelationId);
            }

            var response = await this.client.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new SiestaHttpCallFailedException(response, content);
            }

            if (!string.IsNullOrEmpty(content))
            {
                throw new SiestaContentException(response);
            }

            return Task.CompletedTask;
        }

        private async Task<T> SendRequestWithExpectedContent<T>(HttpRequestMessage requestMessage, string? currentCorrelationId = null)
        {
            if (currentCorrelationId is not null && this.correlationIdHeaderName is not null)
            {
                requestMessage.Headers.Add(this.correlationIdHeaderName, currentCorrelationId);
            }

            var response = await this.client.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new SiestaHttpCallFailedException(response, content);
            }

            try
            {
                var deserialized = JsonConvert.DeserializeObject<T>(content);

                if (deserialized == null)
                {
                    throw new SiestaContentException(response);
                }

                return deserialized;
            }
            catch (SiestaContentException)
            {
                throw;
            }
            catch
            {
                throw new SiestaContentException(response);
            }
        }
    }
}
