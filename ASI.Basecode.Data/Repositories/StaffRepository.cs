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
        /// Get all staff members
        /// </summary>
        /// <returns></returns>
        public IQueryable<RestaurantStaff> GetAllStaff()
        {
            return _dbContext.RestaurantStaff.AsQueryable();
        }

        /// <summary>
        /// Get staff member by ID
        /// </summary>
        /// <param name="staffID"></param>
        /// <returns></returns>
        public RestaurantStaff GetStaffByID(int staffID)
        {
            return _dbContext.RestaurantStaff.FirstOrDefault(s => s.StaffID == staffID);
        }

        /// <summary>
        /// Add a new staff member
        /// </summary>
        /// <param name="staff"></param>
        public void AddStaff(RestaurantStaff staff)
        {
            _dbContext.RestaurantStaff.Add(staff);
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Update an existing staff member
        /// </summary>
        /// <param name="staff"></param>
        public void UpdateStaff(RestaurantStaff staff)
        {
            _dbContext.RestaurantStaff.Update(staff);
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Delete a staff member by ID
        /// </summary>
        /// <param name="staffID"></param>
        /// <exception cref="ArgumentException"></exception>
        public void DeleteStaff(int staffID)
        {
            var staff = GetStaffByID(staffID);
            if (staff != null)
            {
                _dbContext.RestaurantStaff.Remove(staff);
                _dbContext.SaveChanges();
            }
            else
            {
                throw new ArgumentException("Staff not found.");
            }
        }
    }
}