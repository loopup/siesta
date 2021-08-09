namespace LoopUp.Siesta
{
    using System.Threading.Tasks;
    using LoopUp.Siesta.Exceptions;
    using LoopUp.Siesta.RequestConfiguration;

    /// <summary>
    /// Public contract of the <see cref="ISiestaClient"/>.
    /// This can be used to communicate with an API built using Siesta.
    /// </summary>
    public interface ISiestaClient
    {
        /// <summary>
        /// Sends a request that expects no data in return.
        /// </summary>
        /// <param name="siestaRequest">The request to send.</param>
        /// <returns>A completed task.</returns>
        /// <throws><see cref="SiestaHttpException"/>.</throws>
        Task<Task> SendAsync(SiestaRequest siestaRequest);

        /// <summary>
        /// Sends a request that expects data in return.
        /// </summary>
        /// <param name="siestaRequest">The request to send.</param>
        /// <typeparam name="T">The expected return type for data.</typeparam>
        /// <returns>The retrieved, updated or created data.</returns>
        /// <throws><see cref="SiestaHttpException"/>.</throws>
        Task<T> SendAsync<T>(SiestaRequest<T> siestaRequest);

        /// <summary>
        /// Sends a PATCH request that expected data in return.
        /// </summary>
        /// <param name="siestaPatchRequest">The request to send.</param>
        /// <typeparam name="T">The expected return type and the type of the resource being patched.</typeparam>
        /// <returns>The updated resource.</returns>
        /// <throws><see cref="SiestaHttpException"/>.</throws>
        Task<T> SendAsync<T>(SiestaPatchRequest<T> siestaPatchRequest);
    }
}
