namespace ASI.Basecode.Services.ServiceModels
{
    public class AddressViewModel
    {
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public string StreetAddress { get; set; }
        public string Barangay { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
    }
}