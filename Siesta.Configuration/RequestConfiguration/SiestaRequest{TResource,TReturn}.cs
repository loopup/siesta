namespace Siesta.Configuration.RequestConfiguration
{
    using System;
    using System.Net.Http;
    using Siesta.Configuration.Exceptions;

    /// <summary>
    /// Base class for any request that expects data to be returned.
    /// </summary>
    /// <typeparam name="TResource">The type of the resource expected back.</typeparam>
    /// <typeparam name="TReturn">The expected data return type.</typeparam>
    public abstract class SiestaRequest<TResource, TReturn>
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

        /// <summary>
        /// Method to extract the Resource from the Return type which may be different.
        /// </summary>
        /// <param name="returnObject">The return object.</param>
        /// <returns>The resource.</returns>
        public virtual TResource ExtractResourceFromReturn(TReturn returnObject)
        {
            throw new SiestaRequestNotImplementedException();
        }
    }
}
