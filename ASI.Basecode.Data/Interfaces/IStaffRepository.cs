using ASI.Basecode.Data.Models;
using System;
using System.Linq;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IStaffRepository
    {
        IQueryable<RestaurantStaff> GetAllStaff();
        RestaurantStaff GetStaffByID(int staffID);
        void AddStaff(RestaurantStaff staff);
        void UpdateStaff(RestaurantStaff staff);
        void DeleteStaff(int staffID);
    }
}