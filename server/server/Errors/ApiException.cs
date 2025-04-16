namespace server.Errors
{
    /// <summary>
    /// Represents an API exception with additional details about the error
    /// </summary>
    public class ApiException : ApiResponse
    {
        /// <summary>
        /// Creates a new API exception with status code, message, and details
        /// </summary>
        /// <param name="statusCode">The HTTP status code</param>
        /// <param name="message">Optional custom message. If null, a default message will be used</param>
        /// <param name="details">Optional error details such as stack trace or exception information</param>
        public ApiException(int statusCode, string? message = null, string? details = null)
            : base(statusCode, message)
        {
            Details = details;
        }

        /// <summary>
        /// Additional details about the error
        /// </summary>
        public string? Details { get; }

        /// <summary>
        /// Creates a validation exception (400 Bad Request) with details
        /// </summary>
        public static ApiException ValidationError(string? details = null) => 
            new(400, "Validation failed", details);

        /// <summary>
        /// Creates a not found exception (404) with details
        /// </summary>
        public static ApiException NotFound(string? details = null) => 
            new(404, null, details);

        /// <summary>
        /// Creates a server error exception (500) with details
        /// </summary>
        public static ApiException ServerError(string? details = null) => 
            new(500, null, details);
    }
}
