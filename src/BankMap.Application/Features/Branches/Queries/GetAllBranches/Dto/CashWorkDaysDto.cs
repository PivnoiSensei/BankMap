namespace BankMap.Application.Features.Branches.Queries.GetAllBranches.Dto
{
    public record CashWorkDayDto(
        string DayOfWeek,
        string From,
        string To,
        List<BreakIntervalDto> Breaks
    );
}
