using BankMap.Application.Common;
using MediatR;

namespace BankMap.Application.Features.Branches.Commands.UpdateBranchStatus
{
    public record UpdateBranchStatusCommand(int Id, bool IsTemporaryClosed) : IRequest<Result<Unit>>;
}
