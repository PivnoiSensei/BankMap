using System.Text.Json.Serialization;

namespace BankMap.Application.Features.Branches.Commands.ImportJson;

// Root object "list"
public record DepartmentsImportRootDto(
    [property: JsonPropertyName("list")] List<ImportDto> List
);

public record ImportDto(
    [property: JsonPropertyName("departmentId")] int Id,
    [property: JsonPropertyName("departmentName")] string Name,
    [property: JsonPropertyName("departmentType")] string Type,
    bool IsTemporaryClosed,
    bool IsRegular,
    string FullAddress,
    AddressDto Address,
    [property: JsonPropertyName("timeTables")] List<WorkScheduleDto> Schedules,
    [property: JsonPropertyName("phoneNumbers")] List<ContactPhoneDto> Phones,
    [property: JsonPropertyName("cashDepartments")] List<CashDeskDto> CashDesks
);

public record AddressDto(
    string BaseCity,
    string DetailedAddress,
    GeoLocationDto GeoLocation
);

public record GeoLocationDto(
    double Lat,
    [property: JsonPropertyName("long")] double Long
);

public record WorkScheduleDto(
    [property: JsonPropertyName("workstation")] string WorkStation,
    [property: JsonPropertyName("workDays")] List<WorkingDayDto> Days
);

public record WorkingDayDto(
    [property: JsonPropertyName("workingDay")] string DayOfWeek,
    [property: JsonPropertyName("workFrom")] string From,
    [property: JsonPropertyName("workTo")] string To,
    List<BreakIntervalDto> Breaks
);

public record BreakIntervalDto(
    [property: JsonPropertyName("breakFrom")] string From,
    [property: JsonPropertyName("breakTo")] string To
);

public record ContactPhoneDto(
    string OperatorCode,
    [property: JsonPropertyName("phoneNumber")] string Number
);

public record CashDeskDto(
    [property: JsonPropertyName("cashId")] int ExternalId,
    [property: JsonPropertyName("cashDescription")] string Description,
    [property: JsonPropertyName("workDays")] List<CashWorkDayDto> WorkDays
);

public record CashWorkDayDto(
    string DayOfWeek,
    [property: JsonPropertyName("workFrom")] string From,
    [property: JsonPropertyName("workTo")] string To,
    [property: JsonPropertyName("breaks")] List<BreakIntervalDto> Breaks
);