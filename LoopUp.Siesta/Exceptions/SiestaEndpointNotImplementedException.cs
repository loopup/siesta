namespace LoopUp.Siesta.Exceptions
{
    using System;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;

    /// <summary>
    /// Exception thrown when a particular endpoint is not implemented for a DTO.
    /// </summary>
    [Serializable]
    public class SiestaEndpointNotImplementedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SiestaEndpointNotImplementedException"/> class.
        /// </summary>
        public SiestaEndpointNotImplementedException()
            : base("Endpoint not implemented.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SiestaEndpointNotImplementedException"/> class.
        /// </summary>
        /// <param name="innerException">The inner exception.</param>
        public SiestaEndpointNotImplementedException(Exception innerException)
            : base("Endpoint not implemented.", innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SiestaEndpointNotImplementedException"/> class.
        /// </summary>
        /// <param name="info">Instance of <see cref="SerializationInfo"/>.</param>
        /// <param name="context">Instance of <see cref="StreamingContext"/>.</param>
        [JsonConstructor]
        protected SiestaEndpointNotImplementedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
