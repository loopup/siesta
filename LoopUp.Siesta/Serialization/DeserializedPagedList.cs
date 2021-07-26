namespace LoopUp.Siesta.Serialization
{
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;
    using X.PagedList;

    /// <summary>
    /// Extension of Static Paged List used for deserialization of SerializablePagedList.
    /// This class should always be used when deserializing <see cref="SerializablePagedList{T}"/> to preserve it's contents and metadata.
    /// </summary>
    /// <typeparam name="T">Type of the entity stored in set.</typeparam>
    [JsonObject]
    public class DeserializedPagedList<T> : StaticPagedList<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeserializedPagedList{T}"/> class.
        /// </summary>
        /// <param name="items"> IQueryable set of entities that are used to populate list during serialization.</param>
        /// <param name="pageNumber"> Number of page on which data will be displayed.</param>
        /// <param name="pageSize"> Maximum size of the set.</param>
        /// <param name="totalItemCount">Size of the superset that will be paginated.</param>
        public DeserializedPagedList(IQueryable<T> items, int pageNumber, int pageSize, int totalItemCount)
        : base(items, pageNumber, pageSize, totalItemCount)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeserializedPagedList{T}"/> class.
        /// </summary>
        /// <param name="items">IEnumerable set of entities that are used to populate list during serialization.</param>
        /// <param name="pageNumber">Number of page on which data will be displayed.</param>
        /// <param name="pageSize">Maximum size of the set.</param>
        /// <param name="totalItemCount">Size of the superset that will be paginated.</param>
        [JsonConstructor]
        public DeserializedPagedList(IEnumerable<T> items, int pageNumber, int pageSize, int totalItemCount)
            : base(items, pageNumber, pageSize, totalItemCount)
        {
        }

        /// <summary>
        /// Gets dataset of the list that's used for serialization.
        /// </summary>
        public IEnumerable<T> Items => this.Subset;
    }
}
