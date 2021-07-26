namespace LoopUp.Siesta
{
    using System;
    using System.Threading.Tasks;
    using LoopUp.Siesta.DtoConfiguration;
    using LoopUp.Siesta.Serialization;

    /// <summary>
    /// The client consumable Siesta Client.
    /// This can be used when communicating with a REST api built using Siesta.
    /// </summary>
    public interface ISiestaClient
    {
        /// <summary>
        /// Get a resource of type T whose canonical identifier is a Guid.
        /// </summary>
        /// <param name="id">Canonical identifier of resource T.</param>
        /// <typeparam name="T">Expected return DTO type.</typeparam>
        /// <returns>Instance of T.</returns>
        /// <throws>Siesta exception.</throws>
        Task<T> Get<T>(Guid id)
            where T : DtoBase, new();

        /// <summary>
        /// Get a resource of type T whose canonical identifier is an int.
        /// </summary>
        /// <param name="id">Canonical identifier of resource T.</param>
        /// <typeparam name="T">Expected return DTO type.</typeparam>
        /// <returns>Instance of T.</returns>
        /// <throws>Siesta exception.</throws>
        Task<T> Get<T>(int id)
            where T : DtoBase, new();

        /// <summary>
        /// Get a resource of type T whose canonical identifier is a string.
        /// </summary>
        /// <param name="id">Canonical identifier of resource T.</param>
        /// <typeparam name="T">Expected return DTO type.</typeparam>
        /// <returns>Instance of T.</returns>
        /// <throws>Siesta exception.</throws>
        Task<T> Get<T>(string id)
            where T : DtoBase, new();

        /// <summary>
        /// Get a paged list of resource T.
        /// </summary>
        /// <typeparam name="T">Expected return DTO type.</typeparam>
        /// <returns><see cref="DeserializedPagedList{T}"/> of T.</returns>
        Task<DeserializedPagedList<T>> Get<T>()
            where T : DtoBase, new();

        /// <summary>
        /// Get a filtered paged list of resource T.
        /// </summary>
        /// <param name="filterInformation">Filter information to filter the resource by.</param>
        /// <typeparam name="T">Expected return DTO type.</typeparam>
        /// <returns><see cref="DeserializedPagedList{T}"/> of T.</returns>
        Task<DeserializedPagedList<T>> Get<T>(EnumerableFilterInformation filterInformation)
            where T : DtoBase, new();
    }
}
