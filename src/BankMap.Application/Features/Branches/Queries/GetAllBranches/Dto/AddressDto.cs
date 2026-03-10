namespace BankMap.Application.Features.Branches.Queries.GetAllBranches.Dto
{
    public record AddressDto(
         string City,
         string FullAddress,
         string DetailedAddress,
         double Latitude,
         double Longitude
    );
}
