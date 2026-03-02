using BankMap.Application.Common;
using MediatR;

namespace BankMap.Application.Features.Branches.Commands.ImportJson
{
    public record ImportBranchesCommand(Stream JsonStream) : IRequest<Result<int>>;
}
