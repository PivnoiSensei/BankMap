namespace BankMap.Application.Common
{
    public class Result<T>: ResultBase, IResultFailure<Result<T>>
    {
        public T? Value { get; }

        private Result(bool isSuccess, T? value, string? error)
        {
            IsSuccess = isSuccess;
            Value = value;
            Error = error;
        }

        public static Result<T> Success(T value) => new (true, value, null);
        public static Result<T> Failure(string error) => new(false, default, error);
    }
}
