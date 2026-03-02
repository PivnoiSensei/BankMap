using BankMap.Domain.Entities;
using BankMap.Application.Common;

namespace BankMap.Application.Services
{
    public interface IBranchService
    {
        Task<Result<List<Branch>>> GetAllAsync(
            CancellationToken ct);
    }
}
