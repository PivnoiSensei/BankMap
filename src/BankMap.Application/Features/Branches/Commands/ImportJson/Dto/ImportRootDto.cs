using System.Text.Json.Serialization;

namespace BankMap.Application.Features.Branches.Commands.ImportJson.Dto
{
    // Root object "list"
    public record DepartmentsImportRootDto(
        [property: JsonPropertyName("list")] List<ImportDto> List
    );
}
