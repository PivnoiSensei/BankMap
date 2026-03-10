namespace BankMap.Application.Features.Branches.Queries.GetAllBranches.Dto
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
}
