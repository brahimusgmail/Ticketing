namespace Ticketing.Shared.Common
{
    public class ApiResult<T>
    {
        public bool IsSuccess { get; init; }

        public T? Data { get; init; }

        public string? ErrorMessage { get; init; }

        public int StatusCode { get; init; }

        public static ApiResult<T> Success(T data) => new()
        {
            IsSuccess = true,
            Data = data,
            StatusCode = 200,
        };

        public static ApiResult<T> Failure(string error, int statusCode) => new()
        {
            IsSuccess = false,
            ErrorMessage = error,
            StatusCode = statusCode,
        };
    }
}
