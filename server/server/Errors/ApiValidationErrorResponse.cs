namespace server.Errors
{
    public class ApiValidationErrorResponse : ApiResponse
    {
        public ApiValidationErrorResponse() : base(400, "Validation Error", "One or more validation errors occurred")
        {
        }

        public IEnumerable<string> Errors { get; set; }
    }
}
