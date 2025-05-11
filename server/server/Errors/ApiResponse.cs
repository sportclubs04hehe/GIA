namespace server.Errors
{
    public class ApiResponse
    {
        public ApiResponse(int statusCode, string title, string? message = null)
        {
            StatusCode = statusCode;
            Title = title;
            Message = message ?? GetDefaultMessageForStatusCode(statusCode);
            Timestamp = DateTime.UtcNow;
        }

        public int StatusCode { get; }
        public string Title { get; set; }
        public string Message { get; }
        public DateTime Timestamp { get; }

        private string GetDefaultMessageForStatusCode(int statusCode) => statusCode switch
        {
            200 => "Request completed successfully",
            201 => "Resource created successfully",
            400 => "Bad request - the request could not be understood or was missing required parameters",
            404 => "Not found - the requested resource does not exist",
            500 => "Internal server error - something went wrong on our end",
            _ => $"Status code {statusCode}"
        };

        public static ApiResponse Success(string? title = null, string? message = null, bool data = false)
            => new(200, title ?? "Success", message);
        public static ApiResponse BadRequest(string? title = null, string? message = null)
            => new(400, title ?? "Bad Request", message);
        public static ApiResponse NotFound(string? title = null, string? message = null)
            => new(404, title ?? "Not Found", message);
        public static ApiResponse ServerError(string? title = null, string? message = null)
            => new(500, title ?? "Server Error", message);
    }

    // Generic version:
    public class ApiResponse<T> : ApiResponse
    {
        public T? Data { get; set; }
        public object? Errors { get; set; }

        public ApiResponse(int statusCode, string title, string? message = null, T? data = default, object? errors = null)
            : base(statusCode, title, message)
        {
            Data = data;
            Errors = errors;
        }

        public static ApiResponse<T> Success(T data, string? title = null, string? message = null)
            => new(200, title ?? "Success", message, data);

        public static ApiResponse<T> PartialSuccess(T data, string? title = null, string? message = null, object? errors = null)
            => new(207, title ?? "Partial Success", message, data, errors);

        public static ApiResponse<T> Created(T data, string? title = null, string? message = null)
            => new(201, title ?? "Created", message, data);

        public static ApiResponse<T> BadRequest(string? title = null, string? message = null, object? errors = null)
            => new(400, title ?? "Bad Request", message, default, errors);

        public static ApiResponse<T> ServerError(string? title = null, string? message = null)
            => new(500, title ?? "Server Error", message);
    }

}
