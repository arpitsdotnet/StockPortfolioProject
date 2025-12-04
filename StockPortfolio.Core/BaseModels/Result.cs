namespace StockPortfolio.Core.BaseModels
{
    public sealed class Result<T> where T : class
    {
        private Result(T value, string? message)
        {
            Value = value;
            Message = message;
            IsSuccess = true;
        }
        private Result(Error error)
        {
            Error = error;
            IsSuccess = false;
        }

        public bool IsSuccess { get; set; }
        public bool IsFailure => !IsSuccess;
        public Error Error { get; }
        public T Value { get; }
        public string? Message { get; }

        public static Result<T> Success(T value, string? message = null) => new(value, message);
        public static Result<T> Failure(Error error) => new(error);

        public static implicit operator Result<T>(Error error) => Failure(error);
    }
}