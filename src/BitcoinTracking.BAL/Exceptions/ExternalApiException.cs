

namespace BitcoinTracking.BAL.Exceptions
{
    /// <summary>
    /// Represents an exception that is thrown when a call to an external API fails.
    /// This exception is used to distinguish external service errors
    /// from internal application or business logic errors.
    /// HTTP status code for debugging
    /// </summary>
    public class ExternalApiException : Exception
    {
        /// <summary>
        /// Gets the name of the external API that caused the exception
        /// (e.g. "CoinDesk", "CNB").
        /// </summary>
        public string ApiName { get; }

        /// <summary>
        /// Gets the HTTP status code returned by the external API, if available.
        /// This value may be null when the failure occurs before a response is received
        /// (e.g. network error, timeout).
        /// </summary>
        public int? StatusCode { get; }

        /// <summary>
        /// Initializes a new instance of ExternalApiException class
        /// with a specified API name and error message.
        /// </summary>
        /// <param name="apiName">The name of the external API that failed.</param>
        /// <param name="message">A human-readable description of error.</param>
        public ExternalApiException(string apiName, string message)
            : base(message)
        {
            ApiName = apiName;
        }

        /// <summary>
        /// Initializes a new instance of ExternalApiException class
        /// with a specified API name, error message and a reference to inner exception
        /// that caused this error.
        /// </summary>
        /// <param name="apiName">The name of external API that failed.</param>
        /// <param name="message">A human-readable description of error.</param>
        /// <param name="innerException">
        /// The exception that caused current exception, for example thrown by
        /// the HTTP client or serialization process.
        /// </param>
        public ExternalApiException(string apiName, string message, Exception innerException)
            : base(message, innerException)
        {
            ApiName = apiName;
        }


        /// <summary>
        /// Initializes a new instance of ExternalApiException class
        /// with a specified API name, HTTP status code, and error message.
        /// </summary>
        /// <param name="apiName">The name of external API that failed.</param>
        /// <param name="statusCode">HTTP status code returned by external API.</param>
        /// <param name="message">A human-readable description of error.</param>
        public ExternalApiException(string apiName, int statusCode, string message)
            : base(message)
        {
            ApiName = apiName;
            StatusCode = statusCode;
        }


        /// <summary>
        /// Initializes a new instance of ExternalApiException class
        /// with a specified API name, HTTP status code, error message,
        /// and a reference to inner exception that caused this error.
        /// </summary>
        /// <param name="apiName">The name of external API that failed.</param>
        /// <param name="statusCode">HTTP status code returned by external API.</param>
        /// <param name="message">A human-readable description of error.</param>
        /// <param name="innerException">
        /// The exception that caused the current exception, for example thrown by
        /// HTTP client or JSON deserialization.
        /// </param>
        public ExternalApiException(string apiName, int statusCode, string message, Exception innerException)
            : base(message, innerException)
        {
            ApiName = apiName;
            StatusCode = statusCode;
        }
    }
}
