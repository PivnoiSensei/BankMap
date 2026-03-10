namespace BankMap.Application.Features.Branches.Queries.GetAllBranches.Dto
{
    public record WorkScheduleDto(
        string WorkStation,
        List<WorkingDayDto> Days
    );
}
