namespace Siesta.Configuration.Serialization
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
    public class SerializablePagedList<T> : StaticPagedList<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializablePagedList{T}"/> class.
        /// </summary>
        /// <param name="items">IEnumerable set of entities.</param>
        /// <param name="pageNumber">Number of page on which data will be displayed.</param>
        /// <param name="pageSize">Maximum size of the set.</param>
        /// <param name="totalItemCount">Total item count.</param>
        public SerializablePagedList(IEnumerable<T> items, int pageNumber, int pageSize, int totalItemCount)
            : base(items, pageNumber, pageSize, totalItemCount)
        {
        }

        /// <summary>
        /// Gets dataset of the list that's used for serialization.
        /// </summary>
        [JsonProperty]
        public IEnumerable<T> Items => this.Subset;
    }
}
