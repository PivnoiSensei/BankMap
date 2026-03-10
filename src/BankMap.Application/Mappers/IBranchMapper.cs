using BankMap.Application.Common;
using BankMap.Domain.Entities;
using BankMap.Application.Features.Branches.Queries.GetAllBranches.Dto;

namespace BankMap.Application.Mappers
{
    public interface IBranchMapper
    {
        Task<Result<List<Branch>>> MapFromStreamAsync(Stream jsonStream, CancellationToken ct);
        List<BranchDto> MapToDto(List<Branch> branches);
    }
}
