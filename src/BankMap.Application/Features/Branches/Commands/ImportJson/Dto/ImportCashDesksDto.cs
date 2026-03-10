using System.Text.Json.Serialization;

namespace BankMap.Application.Features.Branches.Commands.ImportJson.Dto
{
    public record ImportCashDeskDto(
        [property: JsonPropertyName("cashId")] int ExternalId,
        [property: JsonPropertyName("cashDescription")] string Description,
        [property: JsonPropertyName("workDays")] List<ImportCashWorkDayDto> WorkDays
    );
}
