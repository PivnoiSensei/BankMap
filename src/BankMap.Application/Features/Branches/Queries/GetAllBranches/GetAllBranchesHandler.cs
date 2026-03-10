using MediatR;
using BankMap.Application.Common;
using BankMap.Application.Services;
using BankMap.Application.Features.Branches.Queries.GetAllBranches.Dto;
using BankMap.Application.Mappers;

namespace BankMap.Application.Features.Branches.Queries.GetAllBranches
{
    public class GetAllBranchesHandler : IRequestHandler<GetAllBranchesQuery, Result<List<BranchDto>>>
    {
        private readonly IBranchService _branchService;
        private readonly IBranchMapper _branchMapper; //Маппер для Branch -> BranchDto
        public GetAllBranchesHandler(IBranchService branchService, IBranchMapper branchMapper)
        {
            _branchService = branchService;
            _branchMapper = branchMapper;
        }
        public async Task<Result<List<BranchDto>>> Handle(
            GetAllBranchesQuery req, CancellationToken ct
        )
        {
            var res = await _branchService.GetAllAsync(ct);

            if (!res.IsSuccess)
                if(res.Error != null)
                    return Result<List<BranchDto>>.Failure(res.Error);

            var dtoResult = _branchMapper.MapToDto(res.Value!);

            return Result<List<BranchDto>>.Success(dtoResult);

        }
    }
}
