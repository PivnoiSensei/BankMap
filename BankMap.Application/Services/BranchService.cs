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
    }
}
