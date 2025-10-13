using System;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.WebApp.Areas.Restaurant.Models
{
    public class StaffListItemDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Role { get; set; } = "";
        public string Status { get; set; } = "";
        public string LastLogin { get; set; }
    }

    // Used for Create/Update from the modal
    public class StaffUpsertDto
    {
        public int? Id { get; set; }  // null = create, otherwise update
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public StaffRole Role { get; set; } = StaffRole.Staff;
        public AccountStatus Status { get; set; } = AccountStatus.Active;
        public string Password { get; set; }
    }

    public class StaffDetailsDto : StaffUpsertDto
    {
        public string LastLogin { get; set; } = "—";
    }
}
