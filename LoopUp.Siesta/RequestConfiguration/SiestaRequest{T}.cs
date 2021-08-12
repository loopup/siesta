namespace LoopUp.Siesta.RequestConfiguration
{
    using System;
    using System.Net.Http;
    using LoopUp.Siesta.Exceptions;

    /// <summary>
    /// Base class for any request that expects data to be returned.
    /// </summary>
    /// <typeparam name="T">The expected data return type.</typeparam>
    public abstract class SiestaRequest<T>
    {
        private readonly Type contentType;

        /// <summary>
        /// Initializes a new instance of the <see cref="SiestaRequest{T}"/> class.
        /// </summary>
        protected SiestaRequest() => this.contentType = typeof(T);

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
