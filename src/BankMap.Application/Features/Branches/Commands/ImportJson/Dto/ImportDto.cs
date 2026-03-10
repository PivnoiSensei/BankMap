using System.Text.Json.Serialization;

namespace BankMap.Application.Features.Branches.Commands.ImportJson.Dto;
public record ImportDto(
    [property: JsonPropertyName("departmentId")] int Id,
    [property: JsonPropertyName("departmentName")] string Name,
    [property: JsonPropertyName("departmentType")] string Type,
    bool IsTemporaryClosed,
    bool IsRegular,
    string FullAddress,
    ImportAddressDto Address,
    [property: JsonPropertyName("timeTables")] List<ImportWorkScheduleDto> Schedules,
    [property: JsonPropertyName("phoneNumbers")] List<ImportContactPhoneDto> Phones,
    [property: JsonPropertyName("cashDepartments")] List<ImportCashDeskDto> CashDesks
);
