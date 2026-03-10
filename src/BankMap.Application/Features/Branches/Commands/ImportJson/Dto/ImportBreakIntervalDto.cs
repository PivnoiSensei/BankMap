using System.Text.Json.Serialization;

namespace BankMap.Application.Features.Branches.Commands.ImportJson.Dto
{
    public record ImportBreakIntervalDto(
        [property: JsonPropertyName("breakFrom")] string From,
        [property: JsonPropertyName("breakTo")] string To
    );
}
