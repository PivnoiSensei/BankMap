using BankMap.Application.Common;
using BankMap.Domain.Interfaces.Persistence;
using BankMap.Application.Services;
using MediatR;

namespace BankMap.Application.Features.Branches.Commands.ImportJson;

public class ImportBranchesHandler : IRequestHandler<ImportBranchesCommand, Result<int>>
{
    private readonly IBranchManager _branchManager;
    private readonly IBranchMapper _branchMapper;

    public ImportBranchesHandler(IBranchManager branchManager, IBranchMapper branchMapper)
    {
        _branchManager = branchManager;
        _branchMapper = branchMapper;
    }

    public async Task<Result<int>> Handle(ImportBranchesCommand req, CancellationToken ct)
    {
        var mappingResult = await _branchMapper.MapFromStreamAsync(req.JsonStream, ct);

        if (!mappingResult.IsSuccess)
        {
            return Result<int>.Failure(mappingResult.Error!);
        }

        int importedCount = await _branchManager.ImportBranchesAsync(mappingResult.Value!, ct);

        return Result<int>.Success(importedCount);
    }
}