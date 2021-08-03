using LoopUp.Siesta.RequestConfiguration;

namespace LoopUp.Siesta
{
    using System.Threading.Tasks;
    using LoopUp.Siesta.DtoConfiguration;
    using LoopUp.Siesta.Exceptions;

    /// <summary>
    /// Public contract of the <see cref="ISiestaClient"/>.
    /// This can be used to communicate with an API built using Siesta.
    /// </summary>
    public interface ISiestaClient
    {
        /// <summary>
        /// Sends a DTO request that expects no data in return.
        /// </summary>
        /// <param name="siestaRequest">The DTO request to send.</param>
        /// <returns>A completed task.</returns>
        /// <throws><see cref="SiestaHttpException"/>.</throws>
        Task<Task> SendAsync(SiestaRequestBase siestaRequest);

        /// <summary>
        /// Sends a DTO request that expects data in return.
        /// </summary>
        /// <param name="siestaRequest">The DTO request to send.</param>
        /// <typeparam name="T">The expected return type for data.</typeparam>
        /// <returns>The retrieved, updated or created data.</returns>
        /// /// <throws><see cref="SiestaHttpException"/>.</throws>
        Task<T> SendAsync<T>(SiestaRequestBase<T> siestaRequest);
    }
}
