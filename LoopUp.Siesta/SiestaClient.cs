namespace LoopUp.Siesta
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using LoopUp.Siesta.DtoConfiguration;
    using LoopUp.Siesta.Exceptions;
    using LoopUp.Siesta.Serialization;
    using Microsoft.AspNetCore.WebUtilities;
    using Newtonsoft.Json;

    /// <inheritdoc />
    public class SiestaClient : ISiestaClient
    {
        private readonly HttpClient client;

        /// <summary>
        /// Initializes a new instance of the <see cref="SiestaClient"/> class.
        /// </summary>
        /// <param name="httpClient">A pre configured http client that is set up with the base URL and auth.</param>
        public SiestaClient(HttpClient httpClient) => this.client = httpClient;

        /// <inheritdoc />
        public async Task<T> Get<T>(Guid id)
            where T : DtoBase, new()
            => await this.Get<T>(id.ToString());

        /// <inheritdoc />
        public async Task<T> Get<T>(int id)
            where T : DtoBase, new()
            => await this.Get<T>(id.ToString());

        /// <inheritdoc />
        public async Task<T> Get<T>(string id)
            where T : DtoBase, new()
        {
            var dto = new T();
            if (dto.GetSingleEndpoint == null)
            {
                throw new SiestaEndpointNotImplementedException();
            }

            var response = await this.client.GetAsync($"{dto.GetSingleEndpoint}/{id}");

            if (!response.IsSuccessStatusCode)
            {
                throw new SiestaHttpException("HTTP call was not successful.", response);
            }

            try
            {
                var result = JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());

                if (result == null)
                {
                    throw new SiestaHttpException("Content was not as expected.", response);
                }

                return result;
            }
            catch
            {
                throw new SiestaHttpException("Content was not as expected.", response);
            }
        }

        /// <inheritdoc />
        public async Task<DeserializedPagedList<T>> Get<T>()
            where T : DtoBase, new()
            => await this.GetMany<T>();

        /// <inheritdoc />
        public async Task<DeserializedPagedList<T>> Get<T>(EnumerableFilterInformation filterInformation)
            where T : DtoBase, new()
            => await this.GetMany<T>(filterInformation);

        private async Task<DeserializedPagedList<T>> GetMany<T>(EnumerableFilterInformation? filterInformation = null)
            where T : DtoBase, new()
        {
            var dto = new T();
            if (dto.GetManyEndpoint == null)
            {
                throw new SiestaEndpointNotImplementedException();
            }

            var pathAndQuery = filterInformation != null ?
                QueryHelpers.AddQueryString(dto.GetManyEndpoint, filterInformation.AsQueryDictionary()) :
                dto.GetManyEndpoint;
            var response = await this.client.GetAsync(pathAndQuery);

            if (!response.IsSuccessStatusCode)
            {
                throw new SiestaHttpException("HTTP call was not successful.", response);
            }

            try
            {
                var result = JsonConvert.DeserializeObject<DeserializedPagedList<T>>(await response.Content.ReadAsStringAsync());

                if (result == null)
                {
                    throw new SiestaHttpException("Content was not as expected.", response);
                }

                return result;
            }
            catch
            {
                throw new SiestaHttpException("Content was not as expected.", response);
            }
        }
    }
}
