namespace BankMap.Application.Features.Branches.Queries.GetAllBranches.Dto
{
    public record CashDeskDto(
        int ExternalId,
        string Description,
        List<CashWorkDayDto> WorkDays
    );

}
