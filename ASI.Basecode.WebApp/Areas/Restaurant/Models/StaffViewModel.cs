using System;

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
        public string LastLogin { get; set; } = "—"; // example kay "2 hours ago"
    }

    // Used for Create/Update from the modal
    public class StaffUpsertDto
    {
        public int? Id { get; set; }  // null = create, otherwise update
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string Role { get; set; } = "Cashier";
        public string Status { get; set; } = "Active";
        public DateTime? HireDate { get; set; }
        public decimal? AnnualSalary { get; set; }
        public string? Password { get; set; } //pwede ra dili mo update/edit sa pass
    }

    public class StaffDetailsDto : StaffUpsertDto
    {
        public string LastLogin { get; set; } = "—";
    }
}
