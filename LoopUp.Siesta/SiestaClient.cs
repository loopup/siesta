using LoopUp.Siesta.RequestConfiguration;

namespace LoopUp.Siesta
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using LoopUp.Siesta.DtoConfiguration;
    using LoopUp.Siesta.Exceptions;
    using Newtonsoft.Json;

    /// <summary>
    /// The <see cref="SiestaClient"/> that can be used to communicate with an API built using Siesta.
    /// </summary>
    public class SiestaClient : ISiestaClient
    {
        private readonly HttpClient client;

        /// <summary>
        /// Initializes a new instance of the <see cref="SiestaClient"/> class.
        /// </summary>
        /// <param name="httpClient">A pre-configured <see cref="HttpClient"/>.</param>
        public SiestaClient(HttpClient httpClient)
        {
            this.client = httpClient;
        }

        /// <inheritdoc />
        public async Task<Task> SendAsync(SiestaRequestBase siestaRequest)
        {
            var response = await this.client.SendAsync(siestaRequest.GenerateRequestMessage());

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

        /// <inheritdoc />
        public async Task<T> SendAsync<T>(SiestaRequestBase<T> siestaRequest)
        {
            var response = await this.client.SendAsync(siestaRequest.GenerateRequestMessage());

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
