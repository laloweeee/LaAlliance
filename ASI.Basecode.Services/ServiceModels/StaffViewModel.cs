namespace ASI.Basecode.Services.ServiceModels
{
    public class StaffViewModel
    {
        public int    StaffID       { get; set; }   // or Id—just match how the service reads it
        public string FirstName     { get; set; }
        public string LastName      { get; set; }
        public string Email         { get; set; }
        public string ContactNumber { get; set; }
        public string Role          { get; set; }   // "Admin" | "Staff"

        public string AddressStreet   { get; set; }
        public string AddressBarangay { get; set; }
        public string AddressCity     { get; set; }
        public string AddressProvince { get; set; }
        public string AddressZipCode  { get; set; }

        public string Status { get; set; } = "Active";

        public string Password { get; set; }
    }

    public class StaffList
    {
        public int    Id        { get; set; }
        public string FirstName { get; set; }
        public string LastName  { get; set; }
        public string Email     { get; set; }
        public string Role      { get; set; }
        public string Status    { get; set; }
        public string LastLogin { get; set; }
    }
}
