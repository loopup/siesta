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
        private readonly HttpResponseMessage failedHttpResponseMessage;
        private readonly string? failedMessageContent;

        /// <summary>
        /// Initializes a new instance of the <see cref="SiestaHttpCallFailedException"/> class.
        /// </summary>
        /// <param name="failedHttpResponseMessage">The failed HTTP response.</param>
        /// <param name="failedMessageContent">Content of the failed HTTP response.</param>
        public SiestaHttpCallFailedException(HttpResponseMessage failedHttpResponseMessage, string failedMessageContent = "")
            : base($"HTTP call was unsuccessful. Response: {JsonConvert.SerializeObject(new { failedHttpResponseMessage, failedMessageContent })}")
        {
            this.failedHttpResponseMessage = failedHttpResponseMessage;
            this.failedMessageContent = failedMessageContent;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SiestaHttpCallFailedException"/> class.
        /// </summary>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="failedHttpResponseMessage">The failed HTTP response.</param>
        /// <param name="failedMessageContent">Content of the failed HTTP response.</param>
        public SiestaHttpCallFailedException(Exception innerException, HttpResponseMessage failedHttpResponseMessage, string failedMessageContent = "")
            : base(
                  $"HTTP call was unsuccessful. Response: {JsonConvert.SerializeObject(new { failedHttpResponseMessage, failedMessageContent })}",
                  innerException)
        {
            this.failedHttpResponseMessage = failedHttpResponseMessage;
            this.failedMessageContent = failedMessageContent;
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

            this.failedMessageContent = info.GetString("failedMessageContent");
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
            info.AddValue("failedMessageContent", this.failedMessageContent);

            base.GetObjectData(info, context);
        }
    }
}
