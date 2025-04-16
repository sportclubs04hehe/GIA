using server.Errors;
using System.Net;
using System.Text.Json;

namespace server.Middleware
{
    /// <summary>
    /// Middleware dùng để bắt và xử lý tất cả các exception xảy ra trong quá trình xử lý request.
    /// Nó ghi log lỗi và trả về một response JSON chuẩn với thông tin lỗi.
    /// </summary>
    public class ExceptionMiddleware
    {
        // Delegate đại diện cho middleware tiếp theo trong pipeline.
        private readonly RequestDelegate _next;

        // Dùng để ghi log lỗi (ILogger được inject qua DI).
        private readonly ILogger<ExceptionMiddleware> _logger;

        // Môi trường hiện tại (Development, Production, v.v.).
        private readonly IHostEnvironment _env;

        // Cấu hình JSON serializer: trả về key dưới dạng camelCase.
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        /// <summary>
        /// Hàm khởi tạo middleware, nhận vào delegate tiếp theo, logger và môi trường hệ thống.
        /// </summary>
        /// <param name="next">Middleware kế tiếp trong pipeline</param>
        /// <param name="logger">Logger dùng để ghi lỗi</param>
        /// <param name="env">Biến môi trường để xác định có phải đang ở chế độ Development hay không</param>
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger,
            IHostEnvironment env)
        {
            _env = env;
            _logger = logger;
            _next = next;
        }

        /// <summary>
        /// Hàm chính của middleware. Thực thi middleware kế tiếp, nếu có lỗi thì bắt và xử lý.
        /// </summary>
        /// <param name="context">Ngữ cảnh HTTP hiện tại</param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Gọi middleware tiếp theo
                await _next(context);
            }
            catch (Exception ex)
            {
                // Nếu có lỗi, gọi hàm xử lý lỗi
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Xử lý lỗi: ghi log và trả về response JSON chứa thông tin lỗi.
        /// </summary>
        /// <param name="context">Ngữ cảnh HTTP</param>
        /// <param name="exception">Exception vừa xảy ra</param>
        /// <returns></returns>
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Ghi log lỗi, kèm theo method và path
            _logger.LogError(exception,
                "Request {Method} {Path} failed with error: {ErrorMessage}",
                context.Request.Method,
                context.Request.Path,
                exception.Message);

            // Đặt kiểu nội dung trả về là JSON
            context.Response.ContentType = "application/json";

            // Tạo response dựa trên loại exception
            ApiException response = CreateExceptionResponse(exception);

            // Đặt mã trạng thái HTTP tương ứng
            context.Response.StatusCode = response.StatusCode;

            // Serialize response thành chuỗi JSON với định dạng camelCase
            var json = JsonSerializer.Serialize(response, _jsonOptions);

            // Ghi JSON vào body response
            await context.Response.WriteAsync(json);
        }

        /// <summary>
        /// Dựa vào loại exception, tạo đối tượng ApiException tương ứng với mã lỗi và nội dung.
        /// </summary>
        /// <param name="exception">Exception cần xử lý</param>
        /// <returns>Đối tượng ApiException chứa thông tin lỗi cần trả về</returns>
        private ApiException CreateExceptionResponse(Exception exception)
        {
            // Nếu lỗi là do không tìm thấy file (ví dụ 404)
            if (exception is FileNotFoundException)
            {
                return ApiException.NotFound(
                    _env.IsDevelopment() ? exception.Message : null);
            }

            // Nếu lỗi là không có quyền truy cập (403)
            if (exception is UnauthorizedAccessException)
            {
                return new ApiException(403, "You don't have permission to access this resource",
                    _env.IsDevelopment() ? exception.StackTrace : null);
            }

            // Nếu lỗi do dữ liệu không hợp lệ (400)
            if (exception is ArgumentException or FormatException)
            {
                return ApiException.ValidationError(
                    _env.IsDevelopment() ? exception.Message : null);
            }

            // Lỗi mặc định: lỗi server (500)
            return _env.IsDevelopment()
                ? ApiException.ServerError(exception.StackTrace)
                : ApiException.ServerError();
        }



    }
}
