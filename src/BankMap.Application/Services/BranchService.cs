using BankMap.Application.Common;
using BankMap.Domain.Entities;
using BankMap.Domain.Interfaces.Persistence;

namespace BankMap.Application.Services
{
    public class BranchService : IBranchService
    {
        private readonly IBranchManager _branchManager;

        public BranchService(IBranchManager branchManager)
        {
            _branchManager = branchManager;
        }

        public async Task<Result<List<Branch>>> GetAllAsync(
            CancellationToken ct)
        {
            var branches = await _branchManager.GetAllAsync(ct);

            if (branches.Count == 0)
                return Result<List<Branch>>
                    .Failure("No branches found");

            return Result<List<Branch>>
                .Success(branches);
        }
        public async Task<Result<int>> ImportBranchesAsync(
          List<Branch> branches,
          CancellationToken ct)
        {
            if (branches == null || branches.Count == 0)
                return Result<int>.Failure("Branch list is empty");

            var importedCount = await _branchManager
                .ImportBranchesAsync(branches, ct);

            return Result<int>.Success(importedCount);
        }

        public async Task<Result<bool>> UpdateBranchAsync(
            int id,
            bool isTemporaryClosed,
            CancellationToken ct)
        {
            var updated = await _branchManager
                .UpdateBranchAsync(id, isTemporaryClosed, ct);

            if (!updated)
                return Result<bool>.Failure("Branch not found");

            return Result<bool>.Success(true);
        }
    }
}
