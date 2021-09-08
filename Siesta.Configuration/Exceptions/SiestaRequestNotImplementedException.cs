namespace Siesta.Configuration.Exceptions
{
    using System;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;

    /// <summary>
    /// Exception thrown when the request is not implemented for a DTO.
    /// </summary>
    [Serializable]
    public class SiestaRequestNotImplementedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SiestaRequestNotImplementedException"/> class.
        /// </summary>
        public SiestaRequestNotImplementedException()
            : base("Request not implemented.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SiestaRequestNotImplementedException"/> class.
        /// </summary>
        /// <param name="innerException">The inner exception.</param>
        public SiestaRequestNotImplementedException(Exception innerException)
            : base("Request not implemented.", innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SiestaRequestNotImplementedException"/> class.
        /// </summary>
        /// <param name="info">Instance of <see cref="SerializationInfo"/>.</param>
        /// <param name="context">Instance of <see cref="StreamingContext"/>.</param>
        [JsonConstructor]
        protected SiestaRequestNotImplementedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
