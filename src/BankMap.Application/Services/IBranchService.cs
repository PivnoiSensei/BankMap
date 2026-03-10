using BankMap.Domain.Entities;
using BankMap.Application.Common;

namespace BankMap.Application.Services
{
    public interface IBranchService
    {
        Task<Result<List<Branch>>> GetAllAsync(CancellationToken ct);
        Task<Result<int>> ImportBranchesAsync(List<Branch> branches, CancellationToken ct);
        Task<Result<bool>> UpdateBranchAsync(int id, bool isTemporaryClosed, CancellationToken ct);
    }
}
