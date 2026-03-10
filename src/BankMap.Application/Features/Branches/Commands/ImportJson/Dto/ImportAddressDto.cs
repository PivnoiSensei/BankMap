namespace BankMap.Application.Features.Branches.Commands.ImportJson.Dto
{
    public record ImportAddressDto(
       string BaseCity,
       string DetailedAddress,
       ImportGeoLocationDto GeoLocation
   );
}
