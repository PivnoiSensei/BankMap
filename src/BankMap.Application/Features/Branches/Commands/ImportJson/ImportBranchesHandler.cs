using BankMap.Application.Common;
using BankMap.Application.Mappers;
using BankMap.Application.Services;
using MediatR;

namespace BankMap.Application.Features.Branches.Commands.ImportJson;

public class ImportBranchesHandler : IRequestHandler<ImportBranchesCommand, Result<int>>
{
    private readonly IBranchService _branchService;
    private readonly IBranchMapper _branchMapper; //Branch mapper service for JSON deserialization

    public ImportBranchesHandler(IBranchService branchService, IBranchMapper branchMapper)
    {
        _branchService = branchService;
        _branchMapper = branchMapper;
    }

    public async Task<Result<int>> Handle(ImportBranchesCommand req, CancellationToken ct)
    {
        var mappingResult = await _branchMapper.MapFromStreamAsync(req.JsonStream, ct);

        if (!mappingResult.IsSuccess)
        {
            return Result<int>.Failure(mappingResult.Error!);
        }

        var importRes = await _branchService.ImportBranchesAsync(mappingResult.Value!, ct);

        int importedCount = importRes.Value!;
        return Result<int>.Success(importedCount);
    }
}