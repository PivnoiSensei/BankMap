using System.Text.Json.Serialization;

namespace BankMap.Application.Features.Branches.Commands.ImportJson.Dto
{
    public record ImportCashWorkDayDto(
       string DayOfWeek,
       [property: JsonPropertyName("workFrom")] string From,
       [property: JsonPropertyName("workTo")] string To,
       [property: JsonPropertyName("breaks")] List<ImportBreakIntervalDto> Breaks
   );
}
