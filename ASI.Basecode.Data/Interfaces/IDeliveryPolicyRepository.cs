using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Interfaces
{
    /// <summary>
    /// Interface for DeliveryPolicy Repository
    /// </summary>
    public interface IDeliveryPolicyRepository
    {
        /// <summary>
        /// Add a new delivery policy
        /// </summary>
        /// <param name="deliveryPolicy"></param>
        void AddDeliveryPolicy(DeliveryPolicy deliveryPolicy);

        /// <summary>
        /// Update an existing delivery policy
        /// </summary>
        /// <param name="deliveryPolicy"></param>
        void UpdateDeliveryPolicy(DeliveryPolicy deliveryPolicy);

        /// <summary>
        /// Delete a delivery policy by its ID
        /// </summary>
        /// <param name="policyID"></param>
        void DeleteDeliveryPolicy(int policyID);

        /// <summary>
        /// Get a delivery policy by its ID
        /// </summary>
        /// <param name="policyID"></param>
        /// <returns></returns>
        DeliveryPolicy GetDeliveryPolicyById(int policyID);

        /// <summary>
        /// Get all delivery policies
        /// </summary>
        /// <returns></returns>
        IEnumerable<DeliveryPolicy> GetAllDeliveryPolicies();

        /// <summary>
        /// Get the active delivery policy
        /// </summary>
        /// <returns></returns>
        DeliveryPolicy GetActiveDeliveryPolicy();

        /// <summary>
        /// Get delivery policies by effective date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        IEnumerable<DeliveryPolicy> GetDeliveryPoliciesByDate(DateTime date);
    }
}
