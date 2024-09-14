namespace GoWheels_WebAPI.Utilities
{
    public class OperationResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public int StatusCode { get; set; }
        public object? Data { get; set; }

        public OperationResult(bool success, string? message = null, int statusCode = 200, object? data = null)
        {
            Success = success;
            Message = message;
            StatusCode = statusCode;
            Data = data;
        }
    }
}
