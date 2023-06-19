namespace Siesta.Client
{
    using System.Threading.Tasks;
    using Siesta.Client.Exceptions;
    using Siesta.Configuration.RequestConfiguration;

    /// <summary>
    /// Public contract of the <see cref="ISiestaClient"/>.
    /// This can be used to communicate with an API built using Siesta.
    /// </summary>
    public interface ISiestaClient
    {
        /// <summary>
        /// Sends a request that requires no data in return.
        /// </summary>
        /// <param name="siestaRequest">The request to send.</param>
        /// <returns>A completed task.</returns>
        /// <throws><see cref="SiestaHttpCallFailedException"/>.</throws>
        Task<Task> SendAsync(SiestaRequest siestaRequest);

        /// <summary>
        /// Sends a request that requires no data in return.
        /// </summary>
        /// <param name="siestaRequest">The request to send.</param>
        /// <param name="currentCorrelationId">If you are using a correlation ID and you are already part of a call you can pass this here.</param>
        /// <returns>A completed task.</returns>
        /// <throws><see cref="SiestaHttpCallFailedException"/>.</throws>
        Task<Task> SendAsync(SiestaRequest siestaRequest, string? currentCorrelationId);

        /// <summary>
        /// Sends a request that requires data in return.
        /// </summary>
        /// <param name="siestaRequest">The request to send.</param>
        /// <typeparam name="TResource">The type of the resource.</typeparam>
        /// <typeparam name="TReturn">The expected return type for data.</typeparam>
        /// <returns>The retrieved, updated or created data.</returns>
        /// <throws><see cref="SiestaHttpCallFailedException"/>.</throws>
        Task<TReturn> SendAsync<TResource, TReturn>(SiestaRequest<TResource, TReturn> siestaRequest);

        /// <summary>
        /// Sends a request that requires data in return.
        /// </summary>
        /// <param name="siestaRequest">The request to send.</param>
        /// <param name="currentCorrelationId">If you are using a correlation ID and you are already part of a call you can pass this here.</param>
        /// <typeparam name="TResource">The type of the resource.</typeparam>
        /// <typeparam name="TReturn">The expected return type for data.</typeparam>
        /// <returns>The retrieved, updated or created data.</returns>
        /// <throws><see cref="SiestaHttpCallFailedException"/>.</throws>
        Task<TReturn> SendAsync<TResource, TReturn>(SiestaRequest<TResource, TReturn> siestaRequest, string? currentCorrelationId);

        /// <summary>
        /// Sends a PATCH request that requires data in return.
        /// </summary>
        /// <param name="siestaPatchRequest">The request to send.</param>
        /// <typeparam name="TReturn">The type of the object returned from the PATCH request.</typeparam>
        /// <typeparam name="TResource">The type of the resource being patched.</typeparam>
        /// <typeparam name="TGetReturn">The type of the object returned from the Get request.</typeparam>
        /// <returns>The updated resource.</returns>
        /// <throws><see cref="SiestaHttpCallFailedException"/>.</throws>
        Task<TReturn> SendAsync<TReturn, TResource, TGetReturn>(SiestaPatchRequest<TReturn, TResource, TGetReturn> siestaPatchRequest);

        /// <summary>
        /// Sends a PATCH request that requires data in return.
        /// </summary>
        /// <param name="siestaPatchRequest">The request to send.</param>
        /// <param name="currentCorrelationId">If you are using a correlation ID and you are already part of a call you can pass this here.</param>
        /// <typeparam name="TReturn">The type of the object returned from the PATCH request.</typeparam>
        /// <typeparam name="TResource">The type of the resource being patched.</typeparam>
        /// <typeparam name="TGetReturn">The type of the object returned from the Get request.</typeparam>
        /// <returns>The updated resource.</returns>
        /// <throws><see cref="SiestaHttpCallFailedException"/>.</throws>
        Task<TReturn> SendAsync<TReturn, TResource, TGetReturn>(
            SiestaPatchRequest<TReturn, TResource, TGetReturn> siestaPatchRequest, string? currentCorrelationId);
    }
}
