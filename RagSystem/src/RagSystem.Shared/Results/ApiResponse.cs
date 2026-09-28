using System.Net;

namespace RagSystem.Shared.Results
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public List<string> ErrorMessages { get; set; } = [];
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public static ApiResponse<T> Ok(T data) => new()
        {
            Success = true,
            Data = data,
            StatusCode = HttpStatusCode.OK
        };

        public static ApiResponse<T> Fail(HttpStatusCode code, string message) => new()
        {
            Success = false,
            ErrorMessages = [message],
            StatusCode = code
        };

        public static ApiResponse<T> Fail(HttpStatusCode code, List<string> errors) => new()
        {
            Success = false,
            ErrorMessages = errors,
            StatusCode = code
        };
    }
}
