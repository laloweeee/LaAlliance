using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.Services.Manager;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

// alias to your enums
using Enums = ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Services.Services
{
    /// <summary>
    /// Service for managing restaurant staff operations.
    /// </summary>
    public class StaffService : IStaffService
    {
        private readonly IStaffRepository _staffRepository;

        public StaffService(IStaffRepository staffRepository)
        {
            _staffRepository = staffRepository;
        }

        /// <summary>
        /// Retrieves all restaurant staff members.
        /// </summary>
        public IQueryable<RestaurantStaff> GetAllStaff()
        {
            return _staffRepository.GetAllStaff();
        }

        /// <summary>
        /// Retrieves a staff member by their ID.
        /// </summary>
        public RestaurantStaff GetStaffByID(int staffID)
        {
            return _staffRepository.GetStaffByID(staffID);
        }

        /// <summary>
        /// Adds a new staff member after validating uniqueness of email.
        /// </summary>
        public void AddStaff(StaffViewModel model)
        {
            var existingStaff = _staffRepository.GetAllStaff()
                .FirstOrDefault(s => s.User.Email.ToLower().Trim() == model.Email.ToLower().Trim());

            if (existingStaff != null)
                throw new ArgumentException("A staff member with this email already exists.");

            Enums.StaffRole roleEnum;
            if (!Enum.TryParse(model.Role, ignoreCase: true, out roleEnum))
                roleEnum = Enums.StaffRole.Staff;

            Enums.AccountStatus statusEnum;
            if (!Enum.TryParse(model.Status, ignoreCase: true, out statusEnum))
                statusEnum = Enums.AccountStatus.Active;

            var staff = new RestaurantStaff
            {
                User = new User
                {
                    UserProfile = new UserProfile
                    {
                        FirstName = model.FirstName?.Trim(),
                        LastName = model.LastName?.Trim()
                    },
                    Email = model.Email?.Trim(),
                    UserType = Enums.UserType.Restaurant,
                    IsEmailVerified = true,
                    AccountStatus = statusEnum,
                    Password = PasswordManager.EncryptPassword(model.Password)
                },
                Role = roleEnum,   
                Status = statusEnum  
            };

            _staffRepository.AddStaff(staff);
        }

        /// <summary>
        /// Updates an existing staff member after validating uniqueness of email.
        /// </summary>
        public void UpdateStaff(StaffViewModel model)
        {
            var staff = _staffRepository.GetStaffByID(model.StaffID) ?? throw new ArgumentException("Staff member not found.");

            var existingStaff = _staffRepository.GetAllStaff()
                .FirstOrDefault(s => s.User.Email.ToLower().Trim() == model.Email.ToLower().Trim() && s.StaffID != model.StaffID);

            if (existingStaff != null)
                throw new ArgumentException("A staff member with this email already exists.");

            staff.User.UserProfile.FirstName = model.FirstName?.Trim();
            staff.User.UserProfile.LastName  = model.LastName?.Trim();
            staff.User.Email = model.Email?.Trim();

            if (!string.IsNullOrWhiteSpace(model.Password))
                staff.User.Password = PasswordManager.EncryptPassword(model.Password);

            Enums.StaffRole roleEnum;
            if (!Enum.TryParse(model.Role, ignoreCase: true, out roleEnum))
                roleEnum = Enums.StaffRole.Staff;

            Enums.AccountStatus statusEnum;
            if (!Enum.TryParse(model.Status, ignoreCase: true, out statusEnum))
                statusEnum = Enums.AccountStatus.Active;

            staff.Role = roleEnum;
            staff.Status = statusEnum;
            staff.User.AccountStatus = statusEnum;

            _staffRepository.UpdateStaff(staff);
        }

        public void DeleteStaff(int staffID)
        {
            _staffRepository.DeleteStaff(staffID);
        }

        public async Task<List<StaffList>> ListAsync()
        {
            var list = await _staffRepository.GetAllStaff()
                         .AsNoTracking()
                         .OrderByDescending(s => s.StaffID)
                         .Select(s => new StaffList
                         {
                             Id        = s.StaffID,
                             FirstName = s.User.UserProfile.FirstName,
                             LastName  = s.User.UserProfile.LastName,
                             Email     = s.User.Email,
                             Role      = s.Role.ToString(), 
                             Status    = s.Status.ToString(), 
                             LastLogin = "—"
                         })
                         .ToListAsync();

            return list;
        }
    }
}
