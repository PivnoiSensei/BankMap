namespace BankMap.Domain.Entities
{
    public class AddressInfo
    {
        public string City { get; private set; } = null!;
        public string FullAddress { get; private set; } = null!;
        public string DetailedAddress { get; private set; } = null!;
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }

        private AddressInfo() { }

        public AddressInfo(string city, string fullAddress, string detailedAddress,
            double lat, double lng)
        {
            City = city;
            FullAddress = fullAddress;
            DetailedAddress = detailedAddress;
            Latitude = lat;
            Longitude = lng;
        }
    }
}
