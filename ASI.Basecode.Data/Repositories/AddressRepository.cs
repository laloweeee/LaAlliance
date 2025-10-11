using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace ASI.Basecode.Data.Repositories
{
    /// <summary>
    /// Repository for managing Address entities
    /// </summary>
    public class AddressRepository : BaseRepository, IAddressRepository
    {
        private readonly AsiBasecodeDBContext _dbContext;

        public AddressRepository(IUnitOfWork unitOfWork, AsiBasecodeDBContext dbContext) : base(unitOfWork)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Add a new address
        /// </summary>
        /// <param name="address"></param>
        public void AddAddress(Address address)
        {
            _dbContext.Addresses.Add(address);
            UnitOfWork.SaveChanges();
        }

        /// <summary>
        /// Update an existing address
        /// </summary>
        /// <param name="address"></param>
        public void UpdateAddress(Address address)
        {
            _dbContext.Addresses.Update(address);
            UnitOfWork.SaveChanges();
        }

        /// <summary>
        /// Delete an address by its ID
        /// </summary>
        /// <param name="addressID"></param>
        public void DeleteAddress(int addressID)
        {
            var address = _dbContext.Addresses.Find(addressID);
            if (address != null)
            {
                _dbContext.Addresses.Remove(address);
                UnitOfWork.SaveChanges();
            }
        }

        /// <summary>
        /// Get an address by its ID
        /// </summary>
        /// <param name="addressID"></param>
        /// <returns></returns>
        public Address GetAddressById(int addressID)
        {
            return _dbContext.Addresses
                .Include(a => a.UserAddress)
                .Include(a => a.RestaurantAddress)
                .Include(a => a.Order)
                .FirstOrDefault(a => a.AddressID == addressID);
        }

        /// <summary>
        /// Get all addresses
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Address> GetAllAddresses()
        {
            return _dbContext.Addresses
                .ToList();
        }

        /// <summary>
        /// Get addresses by city
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        public IEnumerable<Address> GetAddressByCity(string city)
        {
            return _dbContext.Addresses
                .Where(a => a.City == city)
                .ToList();
        }

        /// <summary>
        /// Get addresses by province
        /// </summary>
        /// <param name="province"></param>
        /// <returns></returns>
        public IEnumerable<Address> GetAddressByProvince(string province)
        {
            return _dbContext.Addresses
                .Where(a => a.Province == province)
                .ToList();
        }
    }
}
