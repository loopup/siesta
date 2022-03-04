namespace Siesta.Client.Exceptions
{
    using System;
    using System.Net.Http;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;

    /// <summary>
    /// An exception thrown by Siesta client when the content of an HTTP response is not as expected.
    /// </summary>
    [Serializable]
    public class SiestaContentException : Exception
    {
        private HttpResponseMessage httpResponseMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="SiestaContentException"/> class.
        /// </summary>
        /// <param name="httpResponseMessage">The HTTP response.</param>
        public SiestaContentException(HttpResponseMessage httpResponseMessage)
            : base("HTTP response content was not as expected.")
        {
            this.httpResponseMessage = httpResponseMessage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SiestaContentException"/> class.
        /// </summary>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="httpResponseMessage">The HTTP response.</param>
        public SiestaContentException(Exception innerException, HttpResponseMessage httpResponseMessage)
            : base("HTTP response content was not as expected.", innerException)
        {
            this.httpResponseMessage = httpResponseMessage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SiestaContentException"/> class.
        /// </summary>
        /// <param name="info">Instance of <see cref="SerializationInfo"/>.</param>
        /// <param name="context">Instance of <see cref="StreamingContext"/>.</param>
        [JsonConstructor]
        protected SiestaContentException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.httpResponseMessage =
                (HttpResponseMessage)info.GetValue("httpResponseMessage", typeof(HttpResponseMessage)) !;
        }

        /// <summary>
        /// Gets the failed HTTP response.
        /// </summary>
        public HttpResponseMessage HttpResponseMessage => this.httpResponseMessage;

        /// <inheritdoc />
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue("httpResponseMessage", this.httpResponseMessage);
            base.GetObjectData(info, context);
        }
    }
}
