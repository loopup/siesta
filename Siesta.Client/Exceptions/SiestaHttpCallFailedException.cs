namespace Siesta.Client.Exceptions
{
    using System;
    using System.Net.Http;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;

    /// <summary>
    /// An exception thrown by Siesta client when there is an issue with an Http call.
    /// </summary>
    [Serializable]
    public class SiestaHttpCallFailedException : Exception
    {
        private HttpResponseMessage failedHttpResponseMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="SiestaHttpCallFailedException"/> class.
        /// </summary>
        /// <param name="failedHttpResponseMessage">The failed HTTP response.</param>
        public SiestaHttpCallFailedException(HttpResponseMessage failedHttpResponseMessage)
            : base("HTTP call was unsuccessful.")
        {
            this.failedHttpResponseMessage = failedHttpResponseMessage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SiestaHttpCallFailedException"/> class.
        /// </summary>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="failedHttpResponseMessage">The failed HTTP response.</param>
        public SiestaHttpCallFailedException(Exception innerException, HttpResponseMessage failedHttpResponseMessage)
            : base("HTTP call was unsuccessful.", innerException)
        {
            this.failedHttpResponseMessage = failedHttpResponseMessage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SiestaHttpCallFailedException"/> class.
        /// </summary>
        /// <param name="info">Instance of <see cref="SerializationInfo"/>.</param>
        /// <param name="context">Instance of <see cref="StreamingContext"/>.</param>
        [JsonConstructor]
        protected SiestaHttpCallFailedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.failedHttpResponseMessage =
                (HttpResponseMessage)info.GetValue("failedHttpResponseMessage", typeof(HttpResponseMessage)) !;
        }

        /// <summary>
        /// Gets the failed HTTP response.
        /// </summary>
        public HttpResponseMessage FailedHttpResponseMessage => this.failedHttpResponseMessage;

        /// <inheritdoc />
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue("failedHttpResponseMessage", this.failedHttpResponseMessage);
            base.GetObjectData(info, context);
        }
    }
}
