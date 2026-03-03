using BankMap.Application.Common;
using BankMap.Domain.Entities;
using BankMap.Domain.Interfaces.Persistence;
using MediatR;

namespace BankMap.Application.Features.Branches.Commands.UpdateBranchStatus;
public class UpdateBranchStatusHandler : IRequestHandler<UpdateBranchStatusCommand, Result<Unit>>
{
    private readonly IBranchManager _branchManager;

    public UpdateBranchStatusHandler(IBranchManager branchManager)
    {
        _branchManager = branchManager; 
    }

    public async Task<Result<Unit>> Handle(UpdateBranchStatusCommand req, CancellationToken ct)
    {
        var success = await _branchManager.UpdateBranchAsync(req.Id, req.IsTemporaryClosed, ct);
        if (!success)
            return Result<Unit>.Failure("Branch not found");

        return Result<Unit>.Success(Unit.Value);

    }
}

