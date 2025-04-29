using server.Errors;
using System.Net;
using System.Text.Json;

namespace server.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly ILogger<ExceptionMiddleware> _logger;

        private readonly IHostEnvironment _env;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger,
            IHostEnvironment env)
        {
            _env = env;
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception,
                "Request {Method} {Path} failed with error: {ErrorMessage}",
                context.Request.Method,
                context.Request.Path,
                exception.Message);

            context.Response.ContentType = "application/json";

            ApiException response = CreateExceptionResponse(exception);

            context.Response.StatusCode = response.StatusCode;
            var json = JsonSerializer.Serialize(response, _jsonOptions);

            await context.Response.WriteAsync(json);
        }
        private ApiException CreateExceptionResponse(Exception exception)
        {
            if (exception is FileNotFoundException)
            {
                return ApiException.NotFound(
                    _env.IsDevelopment() ? exception.Message : null);
            }

            if (exception is UnauthorizedAccessException)
            {
                return new ApiException(403, "You don't have permission to access this resource",
                    _env.IsDevelopment() ? exception.StackTrace : null);
            }

            if (exception is ArgumentException or FormatException)
            {
                return ApiException.ValidationError(
                    _env.IsDevelopment() ? exception.Message : null);
            }

            return _env.IsDevelopment()
                ? ApiException.ServerError(exception.StackTrace)
                : ApiException.ServerError();
        }
    }
}
