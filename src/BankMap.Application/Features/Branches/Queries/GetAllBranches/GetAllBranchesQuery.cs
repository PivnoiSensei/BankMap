using MediatR;
using BankMap.Application.Common;
using BankMap.Application.Features.Branches.Queries.GetAllBranches.Dto;

namespace BankMap.Application.Features.Branches.Queries.GetAllBranches
{
    public record GetAllBranchesQuery()
        : IRequest<Result<List<BranchDto>>>;
}
