using FluentValidation;

namespace BankMap.Application.Features.Branches.Commands.UpdateBranchStatus
{
    public class UpdateBranchStatusValidator: AbstractValidator<UpdateBranchStatusCommand>
    {
        public UpdateBranchStatusValidator() 
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Branch Id not specified");
        }
    }
}
