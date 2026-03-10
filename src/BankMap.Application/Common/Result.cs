namespace BankMap.Application.Common
{
    public abstract class ResultBase { }
    public class Result<T>: ResultBase
    {
        public bool IsSuccess { get; }
        public T? Value { get; }
        public string? Error { get; }

        private Result(bool isSuccess, T? value, string? error)
        {
            IsSuccess = isSuccess;
            Value = value;
            Error = error;
        }

        public static Result<T> Success(T value) => new (true, value, null);
        public static Result<T> Failure(string error) => new(false, default, error);

        //Новий метод, який повертатиме результат з валідатора
        public static Result<T> FailureStatic(string error)
        {
            return Failure(error)!;
        }
    }
}
