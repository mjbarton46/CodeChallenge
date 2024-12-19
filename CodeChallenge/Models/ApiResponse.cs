using Microsoft.AspNetCore.Http;

namespace CodeChallenge.Models
{
    public class ApiResponse
    {
        public int StatusCode { get; }
        protected ApiResponse(int statusCode)
        {
            StatusCode = statusCode;
        }

        public static readonly ApiResponse NotFound = new(StatusCodes.Status404NotFound);
        public static ApiResponse<T> Ok<T>(T content) => new(content, StatusCodes.Status200OK);
    }

    public class ApiResponse<T> : ApiResponse
    {
        public T Content { get; set; }
        internal ApiResponse(T content, int statusCode): base(statusCode)
        {
            Content = content;
        }
    }
}
