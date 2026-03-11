namespace BankMap.Application.Common
{
    public interface IResultFailure<TSelf>
    {
        static abstract TSelf Failure(string error);
    }
}
