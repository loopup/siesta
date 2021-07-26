namespace LoopUp.Siesta.DtoConfiguration
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// The base which all DTOs received by and returned from Siesta configured REST API's should inherit from.
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public abstract class DtoBase
    {
        /// <summary>
        /// Gets the canonical unique identifier for the resource, normally a guid.
        /// </summary>
        [JsonIgnore]
        public virtual string PrimaryKey => "Id";

        /// <summary>
        /// Gets the collection of reference strings which uniquely identify this resource but are
        /// not considered the canonical identifier for this resource.
        /// </summary>
        [JsonIgnore]
        public virtual IEnumerable<string> ReferenceKeys => new List<string>();

        /// <summary>
        /// Gets the relative path for getting a single resource.
        /// You must override this in order to make use of the get single endpoint.
        /// </summary>
        [JsonIgnore]
        public virtual string? GetSingleEndpoint => null;

        /// <summary>
        /// Gets the relative path for getting an enumerable of the resource.
        /// You must override this in order to make use of the get many endpoints.
        /// </summary>
        [JsonIgnore]
        public virtual string? GetManyEndpoint => null;

        /// <summary>
        /// Gets the relative path for creating a resource.
        /// You must override this in order to make use of the create endpoint.
        /// </summary>
        [JsonIgnore]
        public virtual string? CreateEndpoint => null;

        /// <summary>
        /// Gets the relative path for updating a resource via PUT.
        /// You must override this in order to make use of the PUT endpoint.
        /// </summary>
        [JsonIgnore]
        public virtual string? PutEndpoint => null;

        /// <summary>
        /// Gets the relative path for updating a resource via PATCH.
        /// You must override this in order to make use of the PATCH endpoint.
        /// </summary>
        [JsonIgnore]
        public virtual string? PatchEndpoint => null;

        /// <summary>
        /// Gets the relative path for deleting a single resource.
        /// You must override this in order to make use of the delete endpoint.
        /// </summary>
        [JsonIgnore]
        public virtual string? DeleteEndpoint => null;
    }
}
