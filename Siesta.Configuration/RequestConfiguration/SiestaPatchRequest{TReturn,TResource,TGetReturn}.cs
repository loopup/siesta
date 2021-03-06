namespace Siesta.Configuration.RequestConfiguration
{
    using System.Net.Http;
    using Siesta.Configuration.Exceptions;

    /// <summary>
    /// Base class for a Patch request.
    /// </summary>
    /// <typeparam name="TReturn">The type of object returned by the Patch request and therefore the return type of the overall request.</typeparam>
    /// <typeparam name="TResource">The resource to be patched.</typeparam>
    /// <typeparam name="TGetReturn">The type of the object returned by the Get request.</typeparam>
    public abstract class SiestaPatchRequest<TReturn, TResource, TGetReturn>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SiestaPatchRequest{TResource, TGetReturn, TPatchReturn}"/> class.
        /// </summary>
        /// <param name="modifiedResource">The modified version of the resource to persist through this request.</param>
        protected SiestaPatchRequest(TResource modifiedResource)
        {
            this.ModifiedResource = modifiedResource;
        }

        /// <summary>
        /// Gets or sets the modified resource related to this request.
        /// </summary>
        public TResource ModifiedResource { get; set; }

        /// <summary>
        /// Method that will generate the necessary <see cref="HttpRequestMessage"/> to get the current version of the resource.
        /// You must override this method in order to make a request using this request model.
        /// </summary>
        /// <returns>The <see cref="HttpRequestMessage"/>.</returns>
        public virtual HttpRequestMessage GenerateGetRequestMessage()
        {
            throw new SiestaRequestNotImplementedException();
        }

        /// <summary>
        /// Method that will generate the necessary <see cref="HttpRequestMessage"/> from the request model.
        /// You must override this method in order to make a request using this request model.
        /// </summary>
        /// <param name="originalResource">The original version of the resource. If used in conjunction with the Siesta client this will be provided.</param>
        /// <returns>The <see cref="HttpRequestMessage"/>.</returns>
        public virtual HttpRequestMessage GeneratePatchRequestMessage(TResource originalResource)
        {
            throw new SiestaRequestNotImplementedException();
        }

        /// <summary>
        /// Method to extract the Resource from the Get Return type which may be different.
        /// </summary>
        /// <param name="getReturnObject">The Get return object.</param>
        /// <returns>The resource.</returns>
        public virtual TResource ExtractResourceFromGetReturn(TGetReturn getReturnObject)
        {
            throw new SiestaRequestNotImplementedException();
        }

        /// <summary>
        /// Method to extract the Resource from the Patch Return type which may be different.
        /// </summary>
        /// <param name="patchReturnObject">The PATCH return object.</param>
        /// <returns>The resource.</returns>
        public virtual TResource ExtractResourceFromReturn(TReturn patchReturnObject)
        {
            throw new SiestaRequestNotImplementedException();
        }
    }
}
