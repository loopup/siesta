namespace LoopUp.Siesta.Serialization
{
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;
    using X.PagedList;

    /// <summary>
    /// Extension of PagedList that allows serialization of entity set while preserving it's metadata.
    /// Use <see cref="DeserializedPagedList{T}"/> to deserialize this class.
    /// </summary>
    /// <typeparam name="T">Type of the entity stored in set.</typeparam>
    [JsonObject]
    public class SerializablePagedList<T> : PagedList<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializablePagedList{T}"/> class.
        /// </summary>
        /// <param name="items">IQueryable set of entities.</param>
        /// <param name="pageNumber">Number of page on which data will be displayed.</param>
        /// <param name="pageSize">Maximum size of the set.</param>
        public SerializablePagedList(IQueryable<T> items, int pageNumber, int pageSize)
        : base(items, pageNumber, pageSize)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializablePagedList{T}"/> class.
        /// </summary>
        /// <param name="items">IEnumerable set of entities.</param>
        /// <param name="pageNumber">Number of page on which data will be displayed.</param>
        /// <param name="pageSize">Maximum size of the set.</param>
        public SerializablePagedList(IEnumerable<T> items, int pageNumber, int pageSize)
            : base(items, pageNumber, pageSize)
        {
        }

        /// <summary>
        /// Gets dataset of the list that's used for serialization.
        /// </summary>
        [JsonProperty]
        public IEnumerable<T> Items => this.Subset;
    }
}
