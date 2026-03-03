using BankMap.Domain.Entities;

namespace BankMap.Domain.Interfaces.Persistence
{
  public interface IBranchManager
    {
        Task<List<Branch>> GetAllAsync(CancellationToken ct);
        Task<int> ImportBranchesAsync(List<Branch> list, CancellationToken ct);
        Task<bool> UpdateBranchAsync(int id, bool isTemporaryClosed, CancellationToken ct);
    }
}
