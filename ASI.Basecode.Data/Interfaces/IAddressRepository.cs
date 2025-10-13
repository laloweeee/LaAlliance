using ASI.Basecode.Data.Models;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Interfaces
{
    /// <summary>
    /// Interface for Address Repository
    /// </summary>
    public interface IAddressRepository
    {
        /// <summary>
        /// Add a new address
        /// </summary>
        /// <param name="address"></param>
        void AddAddress(Address address);

        /// <summary>
        /// Update an existing address
        /// </summary>
        /// <param name="address"></param>
        void UpdateAddress(Address address);

        /// <summary>
        /// Delete an address by its ID
        /// </summary>
        /// <param name="addressID"></param>
        void DeleteAddress(int addressID);

        /// <summary>
        /// Get an address by its ID
        /// </summary>
        /// <param name="addressID"></param>
        /// <returns></returns>
        Address GetAddressById(int addressID);

        /// <summary>
        /// Get all addresses
        /// </summary>
        /// <returns></returns>
        IEnumerable<Address> GetAllAddresses();

        /// <summary>
        /// Get addresses by city
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        IEnumerable<Address> GetAddressByCity(string city);

        /// <summary>
        /// Get addresses by province
        /// </summary>
        /// <param name="province"></param>
        /// <returns></returns>
        IEnumerable<Address> GetAddressByProvince(string province);
    }
}
