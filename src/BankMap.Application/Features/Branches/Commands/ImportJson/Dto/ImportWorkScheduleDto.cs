using System.Text.Json.Serialization;

namespace BankMap.Application.Features.Branches.Commands.ImportJson.Dto
{
    public record ImportWorkScheduleDto(
        [property: JsonPropertyName("workstation")] string WorkStation,
        [property: JsonPropertyName("workDays")] List<ImportWorkingDayDto> Days
    );
}
