namespace server.Errors
{
    public class ApiException : ApiResponse
    {
        public ApiException(int statusCode, string? message = null, string? details = null)
            : base(statusCode, message)
        {
            Details = details;
        }

        public string? Details { get; }
        public static ApiException ValidationError(string? details = null) => 
            new(400, "Validation failed", details);

        public static ApiException NotFound(string? details = null) => 
            new(404, null, details);

        public static ApiException ServerError(string? details = null) => 
            new(500, null, details);
    }
}
