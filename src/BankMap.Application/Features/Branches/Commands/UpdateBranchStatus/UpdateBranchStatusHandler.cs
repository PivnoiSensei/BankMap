using BankMap.Application.Common;
using BankMap.Application.Services;
using MediatR;

namespace BankMap.Application.Features.Branches.Commands.UpdateBranchStatus;
public class UpdateBranchStatusHandler : IRequestHandler<UpdateBranchStatusCommand, Result<Unit>>
{
    private readonly IBranchService _branchService;

    public UpdateBranchStatusHandler(IBranchService branchService)
    {
        _branchService = branchService; 
    }

    public async Task<Result<Unit>> Handle(UpdateBranchStatusCommand req, CancellationToken ct)
    {
        var res = await _branchService.UpdateBranchAsync(req.Id, req.IsTemporaryClosed, ct);
        if (!res.IsSuccess)
            return Result<Unit>.Failure("Branch not found");

        return Result<Unit>.Success(Unit.Value);

    }
}

