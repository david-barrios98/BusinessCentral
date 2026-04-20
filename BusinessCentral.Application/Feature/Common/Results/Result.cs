
namespace BusinessCentral.Application.Feature.Common.Results
{
    public class Result<T>
    {
        public bool IsSuccess { get; private set; }
        public T? Value { get; private set; }
        public string? Error { get; private set; }
        public string? ErrorCode { get; private set; } // Ej: MessageKeys.USER_NOT_FOUND
        public string ErrorType { get; private set; } // Ej: "NotFound", "Validation", "Conflict"

        private Result(bool isSuccess, T? value, string? error, string? errorCode, string errorType)
        {
            IsSuccess = isSuccess;
            Value = value;
            Error = error;
            ErrorCode = errorCode;
            ErrorType = errorType;
        }

        public static Result<T> Success(T value)
            => new(true, value, null, null, "Success");

        public static Result<T> Failure(string error, string errorCode, string errorType = "BadRequest")
            => new(false, default, error, errorCode, errorType);
    }
}