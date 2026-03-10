using System.Text.Json.Serialization;

namespace BankMap.Application.Features.Branches.Commands.ImportJson.Dto
{
    public record ImportWorkingDayDto(
        [property: JsonPropertyName("workingDay")] string DayOfWeek,
        [property: JsonPropertyName("workFrom")] string From,
        [property: JsonPropertyName("workTo")] string To,
        List<ImportBreakIntervalDto> Breaks
    );
}
