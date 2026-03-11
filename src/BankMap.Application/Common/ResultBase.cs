namespace BankMap.Application.Common
{
    public abstract class ResultBase 
    {
        public bool IsSuccess { get; protected set; }
        public string? Error { get; protected set; }
    }
}
