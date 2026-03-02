namespace BankMap.Application.Features.Branches.Queries.GetAllBranches
{
    public record BranchDto(
         int Id,
         string Name,
         string Type,
         bool IsTemporaryClosed,
         bool IsRegular,
         AddressDto Address,
         List<WorkScheduleDto> Schedules,
         List<ContactPhoneDto> Phones,
         List<CashDeskDto> CashDesks
     );

    public record AddressDto(
        string City,
        string FullAddress,
        string DetailedAddress,
        double Latitude,
        double Longitude
    );

    public record WorkScheduleDto(
        string WorkStation,
        List<WorkingDayDto> Days
    );

    public record WorkingDayDto(
        string DayOfWeek,
        string From,
        string To,
        List<BreakIntervalDto> Breaks
    );

    public record BreakIntervalDto(
        string From,
        string To
    );

    public record ContactPhoneDto(
        string OperatorCode,
        string Number,
        string FullNumber
    );

    public record CashDeskDto(
        int ExternalId,
        string Description,
        List<CashWorkDayDto> WorkDays
    );

    public record CashWorkDayDto(
        string DayOfWeek,
        string From,
        string To,
        List<BreakIntervalDto> Breaks
    );
}
