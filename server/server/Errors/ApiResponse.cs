namespace server.Errors
{
    /// <summary>
    /// Represents a standardized API response with status code and message
    /// </summary>
    /// <summary>
    /// Đại diện cho một phản hồi API chuẩn hóa với mã trạng thái và thông điệp
    /// </summary>
    public class ApiResponse
    {
        /// <summary>
        /// Khởi tạo một đối tượng ApiResponse mới
        /// </summary>
        /// <param name="statusCode">Mã trạng thái HTTP</param>
        /// <param name="message">Thông điệp tùy chỉnh (tùy chọn). Nếu null, một thông điệp mặc định sẽ được sử dụng.</param>
        public ApiResponse(int statusCode, string? message = null)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultMessageForStatusCode(statusCode);
            Timestamp = DateTime.UtcNow;
        }

        /// <summary>
        /// Lấy mã trạng thái HTTP
        /// </summary>
        public int StatusCode { get; private set; }

        /// <summary>
        /// Lấy thông điệp phản hồi
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Lấy thời gian phản hồi được tạo ra
        /// </summary>
        public DateTime Timestamp { get; private set; }

        /// <summary>
        /// Trả về thông điệp mặc định cho mã trạng thái HTTP đã cho
        /// </summary>
        private string GetDefaultMessageForStatusCode(int statusCode)
        {
            return statusCode switch
            {
                200 => "Request completed successfully",
                201 => "Resource created successfully",
                204 => "Request completed successfully with no content to return",
                400 => "Bad request - the request could not be understood or was missing required parameters",
                401 => "Unauthorized - authentication required",
                403 => "Forbidden - you don't have permission to access this resource",
                404 => "Not found - the requested resource does not exist",
                405 => "Method not allowed for the requested resource",
                409 => "Conflict with the current state of the resource",
                422 => "Validation error - the request was well-formed but contained semantic errors",
                429 => "Too many requests - please try again later",
                500 => "Internal server error - something went wrong on our end",
                502 => "Bad gateway - the server received an invalid response from an upstream server",
                503 => "Service unavailable - the server is currently unable to handle the request",
                _ => $"Status code {statusCode}"
            };
        }

           /// <summary>
        /// Tạo phản hồi thành công (200 OK)
        /// </summary>
        public static ApiResponse Success(string? message = null) => new(200, message);

        /// <summary>
        /// Tạo phản hồi không tìm thấy (404)
        /// </summary>
        public static ApiResponse NotFound(string? message = null) => new(404, message);

        /// <summary>
        /// Tạo phản hồi yêu cầu không hợp lệ (400)
        /// </summary>
        public static ApiResponse BadRequest(string? message = null) => new(400, message);

        /// <summary>
        /// Tạo phản hồi không được phép (401)
        /// </summary>
        public static ApiResponse Unauthorized(string? message = null) => new(401, message);

        /// <summary>
        /// Tạo phản hồi lỗi máy chủ (500)
        /// </summary>
        public static ApiResponse ServerError(string? message = null) => new(500, message);
    }

}
