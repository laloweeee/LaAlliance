using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Services.ServiceModels
{
    public class StaffViewModel
    {
        public int StaffID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public StaffRole Role { get; set; } = StaffRole.Staff;
        public AccountStatus Status { get; set; } = AccountStatus.Active;
        public string Password { get; set; }
    }
}