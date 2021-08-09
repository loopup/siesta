#pragma warning disable SA1649
namespace LoopUp.Siesta.RequestConfiguration
{
    using System;

    /// <summary>
    /// Base class for any request that expects data to be returned.
    /// </summary>
    /// <typeparam name="T">The expected data return type.</typeparam>
    public abstract class SiestaRequest<T> : SiestaRequest
    {
        private readonly Type contentType;

        /// <summary>
        /// Initializes a new instance of the <see cref="SiestaRequest{T}"/> class.
        /// </summary>
        protected SiestaRequest() => this.contentType = typeof(T);
    }
}
#pragma warning restore SA1649
