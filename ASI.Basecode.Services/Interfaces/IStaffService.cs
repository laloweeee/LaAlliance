using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IStaffService
    {
        IQueryable<RestaurantStaff> GetAllStaff();
        RestaurantStaff GetStaffByID(int staffID);
        void AddStaff(StaffViewModel model);
        void UpdateStaff(StaffViewModel model);
        void DeleteStaff(int staffID);
    }
}