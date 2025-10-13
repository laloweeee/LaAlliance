using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.Services.Manager;
using System;
using System.Linq;


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
        /// <returns></returns>
        public IQueryable<RestaurantStaff> GetAllStaff()
        {
            return _staffRepository.GetAllStaff();
        }

        /// <summary>
        /// Retrieves a staff member by their ID.
        /// </summary>
        /// <param name="staffID"></param>
        /// <returns></returns>
        public RestaurantStaff GetStaffByID(int staffID)
        {
            return _staffRepository.GetStaffByID(staffID);
        }

        /// <summary>
        /// Adds a new staff member after validating uniqueness of email.
        /// </summary>
        /// <param name="model"></param>
        /// <exception cref="ArgumentException"></exception>
        public void AddStaff(StaffViewModel model)
        {
            var existingStaff = _staffRepository.GetAllStaff()
                .FirstOrDefault(s => s.User.Email.ToLower().Trim() == model.Email.ToLower().Trim());

            if (existingStaff != null)
            {
                throw new ArgumentException("A staff member with this email already exists.");
            }

            var staff = new RestaurantStaff
            {
                User = new User
                {
                    UserProfile = new UserProfile
                    {
                        FirstName = model.FirstName.Trim(),
                        LastName = model.LastName.Trim()
                    },
                    Email = model.Email.Trim(),
                    UserType = Resources.Constants.Enums.UserType.Restaurant,
                    IsEmailVerified = true,
                    AccountStatus = Resources.Constants.Enums.AccountStatus.Active,
                    Password = PasswordManager.EncryptPassword(model.Password)
                },
                Role = model.Role,
                Status = model.Status,
            };

            _staffRepository.AddStaff(staff);
        }

        /// <summary>
        /// Updates an existing staff member after validating uniqueness of email.
        /// </summary>
        /// <param name="model"></param>
        /// <exception cref="ArgumentException"></exception>
        public void UpdateStaff(StaffViewModel model)
        {
            var staff = _staffRepository.GetStaffByID(model.StaffID) ?? throw new ArgumentException("Staff member not found.");

            var existingStaff = _staffRepository.GetAllStaff()
                .FirstOrDefault(s => s.User.Email.ToLower().Trim() == model.Email.ToLower().Trim() && s.StaffID != model.StaffID);

            if (existingStaff != null)
            {
                throw new ArgumentException("A staff member with this email already exists.");
            }

            staff.User.UserProfile.FirstName = model.FirstName.Trim();
            staff.User.UserProfile.LastName = model.LastName.Trim();
            staff.User.Email = model.Email.Trim();
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                staff.User.Password = PasswordManager.EncryptPassword(model.Password);
            }
            staff.Role = model.Role;
            staff.Status = model.Status;

            _staffRepository.UpdateStaff(staff);
        }        
        public void DeleteStaff(int staffID)
        {
            _staffRepository.DeleteStaff(staffID);
        }
    }
}