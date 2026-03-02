using MediatR;
using BankMap.Application.Common;

namespace BankMap.Application.Features.Branches.Queries.GetAllBranches
{
    public record GetAllBranchesQuery()
        : IRequest<Result<List<BranchDto>>>;
}
