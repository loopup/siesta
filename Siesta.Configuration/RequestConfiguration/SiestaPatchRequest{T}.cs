namespace Siesta.Configuration.RequestConfiguration
{
    using System.Net.Http;
    using Siesta.Configuration.Exceptions;

    /// <summary>
    /// Base class for a Patch request.
    /// </summary>
    /// /// <typeparam name="T">The expected data return type.</typeparam>
    public abstract class SiestaPatchRequest<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SiestaPatchRequest{T}"/> class.
        /// </summary>
        /// <param name="modifiedResource">The modified version of the resource to persist through this request.</param>
        protected SiestaPatchRequest(T modifiedResource)
        {
            this.ModifiedResource = modifiedResource;
        }

        /// <summary>
        /// Gets or sets the modified resource related to this request.
        /// </summary>
        public T ModifiedResource { get; set; }

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
        public virtual HttpRequestMessage GeneratePatchRequestMessage(T originalResource)
        {
            throw new SiestaRequestNotImplementedException();
        }
    }
}
