namespace Siesta.Configuration.Exceptions
{
    using System;
    using System.Net.Http;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;

    /// <summary>
    /// An exception thrown by Siesta client when there is an issue with an Http call.
    /// </summary>
    [Serializable]
    public class SiestaHttpException : Exception
    {
        private HttpResponseMessage? failedHttpResponseMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="SiestaHttpException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="failedHttpResponseMessage">The failed HTTP response.</param>
        public SiestaHttpException(string message, HttpResponseMessage failedHttpResponseMessage)
            : base(message)
        {
            this.failedHttpResponseMessage = failedHttpResponseMessage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SiestaHttpException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="failedHttpResponseMessage">The failed HTTP response.</param>
        public SiestaHttpException(string message, Exception innerException, HttpResponseMessage failedHttpResponseMessage)
            : base(message, innerException)
        {
            this.failedHttpResponseMessage = failedHttpResponseMessage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SiestaHttpException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public SiestaHttpException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SiestaHttpException"/> class.
        /// </summary>
        /// <param name="info">Instance of <see cref="SerializationInfo"/>.</param>
        /// <param name="context">Instance of <see cref="StreamingContext"/>.</param>
        [JsonConstructor]
        protected SiestaHttpException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.failedHttpResponseMessage =
                (HttpResponseMessage)info.GetValue("FailedHttpResponseMessage", typeof(HttpResponseMessage)) !;
        }

        /// <summary>
        /// Gets the failed HTTP response.
        /// </summary>
        public HttpResponseMessage? FailedHttpResponseMessage => this.failedHttpResponseMessage;

        /// <inheritdoc />
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue("FailedHttpResponseMessage", this.FailedHttpResponseMessage);
            base.GetObjectData(info, context);
        }
    }
}
