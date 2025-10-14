using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace ASI.Basecode.Data.Repositories
{
    public class StaffRepository : BaseRepository, IStaffRepository
    {
        private readonly AsiBasecodeDBContext _dbContext;
        public StaffRepository(IUnitOfWork unitOfWork, AsiBasecodeDBContext dbContext) : base(unitOfWork)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Get all staff members with related data
        /// </summary>
        public IQueryable<RestaurantStaff> GetAllStaff()
        {
            return _dbContext.RestaurantStaff
                .Include(s => s.User)
                    .ThenInclude(u => u.UserProfile)
                .AsQueryable();
        }

        /// <summary>
        /// Get staff member by ID with related User and UserProfile data
        /// </summary>
        public RestaurantStaff GetStaffByID(int staffID)
        {
            // FIX: Include User and UserProfile so the data is loaded!
            return _dbContext.RestaurantStaff
                .Include(s => s.User)
                    .ThenInclude(u => u.UserProfile)
                .FirstOrDefault(s => s.StaffID == staffID);
        }

        /// <summary>
        /// Add a new staff member
        /// </summary>
        public void AddStaff(RestaurantStaff staff)
        {
            _dbContext.RestaurantStaff.Add(staff);
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Update an existing staff member
        /// </summary>
        public void UpdateStaff(RestaurantStaff staff)
        {
            // FIX: Attach and mark as modified to ensure updates work
            var existingStaff = _dbContext.RestaurantStaff
                .Include(s => s.User)
                    .ThenInclude(u => u.UserProfile)
                .FirstOrDefault(s => s.StaffID == staff.StaffID);

            if (existingStaff != null)
            {
                // Update RestaurantStaff properties
                existingStaff.Role = staff.Role;
                existingStaff.Status = staff.Status;
                
                // Update User properties
                existingStaff.User.Email = staff.User.Email;
                existingStaff.User.AccountStatus = staff.User.AccountStatus;
                
                // Update password only if it was changed
                if (!string.IsNullOrWhiteSpace(staff.User.Password))
                {
                    existingStaff.User.Password = staff.User.Password;
                }
                
                // Update UserProfile properties
                existingStaff.User.UserProfile.FirstName = staff.User.UserProfile.FirstName;
                existingStaff.User.UserProfile.LastName = staff.User.UserProfile.LastName;
                existingStaff.User.UserProfile.ContactNumber = staff.User.UserProfile.ContactNumber;

                _dbContext.SaveChanges();
            }
        }

        /// <summary>
        /// Delete a staff member by ID (manually cascade delete User and UserProfile)
        /// </summary>
        public void DeleteStaff(int staffID)
        {
            var staff = _dbContext.RestaurantStaff
                .Include(s => s.User)
                    .ThenInclude(u => u.UserProfile)
                .FirstOrDefault(s => s.StaffID == staffID);

            if (staff != null)
            {
                // Manually cascade delete in the correct order
                
                // 1. Delete RestaurantStaff record first (has FK to User)
                _dbContext.RestaurantStaff.Remove(staff);
                
                // 2. Delete UserProfile if exists (has FK to User)
                if (staff.User?.UserProfile != null)
                {
                    _dbContext.UserProfiles.Remove(staff.User.UserProfile);
                }
                
                // 3. Delete User record last
                if (staff.User != null)
                {
                    _dbContext.Users.Remove(staff.User);
                }
                
                _dbContext.SaveChanges();
            }
            else
            {
                throw new ArgumentException("Staff not found.");
            }
        }
    }
}