using System.Text.Json.Serialization;

namespace BankMap.Application.Features.Branches.Commands.ImportJson.Dto
{
    public record ImportGeoLocationDto(
        double Lat,
        [property: JsonPropertyName("long")] double Long
    );

}
