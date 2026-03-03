using BankMap.Application.Common;
using BankMap.Domain.Entities;

namespace BankMap.Application.Services
{
    public interface IBranchMapper
    {
        Task<Result<List<Branch>>> MapFromStreamAsync(Stream jsonStream, CancellationToken ct);
    }
}
