using FluentValidation;

namespace BankMap.Application.Features.Branches.Commands.ImportJson
{
    public class ImportBranchesValidator : AbstractValidator<ImportBranchesCommand>
    {
        public ImportBranchesValidator()
        {
            RuleFor(x => x.JsonStream)
                .NotNull()
                .WithMessage("JSON file stream is required")
                .Must(stream => stream.Length > 0)
                .WithMessage("JSON file stream is empty");
        }
    }
}
