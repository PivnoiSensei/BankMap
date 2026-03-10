using System.Text.Json.Serialization;

namespace BankMap.Application.Features.Branches.Commands.ImportJson.Dto
{
    public record ImportContactPhoneDto(
        string OperatorCode,
        [property: JsonPropertyName("phoneNumber")] string Number
    );
}
