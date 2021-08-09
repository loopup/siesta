namespace LoopUp.Siesta.RequestConfiguration
{
    using System.Net.Http;
    using LoopUp.Siesta.Exceptions;

    /// <summary>
    /// Base class for any request that expects no data to be returned.
    /// </summary>
    public abstract class SiestaRequest
    {
        /// <summary>
        /// Method that will generate the necessary <see cref="HttpRequestMessage"/> from the request model.
        /// You must override this method in order to make a request using this request model.
        /// </summary>
        /// <returns>The <see cref="HttpRequestMessage"/>.</returns>
        public virtual HttpRequestMessage GenerateRequestMessage()
        {
            throw new SiestaRequestNotImplementedException();
        }
    }
}
