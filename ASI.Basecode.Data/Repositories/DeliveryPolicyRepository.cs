using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASI.Basecode.Data.Repositories
{
    /// <summary>
    /// Repository for managing DeliveryPolicy entities
    /// </summary>
    public class DeliveryPolicyRepository : BaseRepository, IDeliveryPolicyRepository
    {
        private readonly AsiBasecodeDBContext _dbContext;

        public DeliveryPolicyRepository(IUnitOfWork unitOfWork, AsiBasecodeDBContext dbContext) : base(unitOfWork)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Add a new delivery policy
        /// </summary>
        /// <param name="deliveryPolicy"></param>
        public void AddDeliveryPolicy(DeliveryPolicy deliveryPolicy)
        {
            _dbContext.DeliveryPolicies.Add(deliveryPolicy);
            UnitOfWork.SaveChanges();
        }

        /// <summary>
        /// Update an existing delivery policy
        /// </summary>
        /// <param name="deliveryPolicy"></param>
        public void UpdateDeliveryPolicy(DeliveryPolicy deliveryPolicy)
        {
            _dbContext.DeliveryPolicies.Update(deliveryPolicy);
            UnitOfWork.SaveChanges();
        }

        /// <summary>
        /// Delete a delivery policy by its ID
        /// </summary>
        /// <param name="policyID"></param>
        public void DeleteDeliveryPolicy(int policyID)
        {
            var deliveryPolicy = _dbContext.DeliveryPolicies.Find(policyID);
            if (deliveryPolicy != null)
            {
                _dbContext.DeliveryPolicies.Remove(deliveryPolicy);
                UnitOfWork.SaveChanges();
            }
        }

        /// <summary>
        /// Get a delivery policy by its ID
        /// </summary>
        /// <param name="policyID"></param>
        /// <returns></returns>
        public DeliveryPolicy GetDeliveryPolicyById(int policyID)
        {
            return _dbContext.DeliveryPolicies
                .FirstOrDefault(dp => dp.PolicyID == policyID);
        }

        /// <summary>
        /// Get all delivery policies
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DeliveryPolicy> GetAllDeliveryPolicies()
        {
            return _dbContext.DeliveryPolicies
                .OrderByDescending(dp => dp.EffectiveDate)
                .ToList();
        }

        /// <summary>
        /// Get the active delivery policy
        /// </summary>
        /// <returns></returns>
        public DeliveryPolicy GetActiveDeliveryPolicy()
        {
            return _dbContext.DeliveryPolicies
                .Where(dp => dp.IsActive)
                .OrderByDescending(dp => dp.EffectiveDate)
                .FirstOrDefault();
        }

        /// <summary>
        /// Get delivery policies by effective date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public IEnumerable<DeliveryPolicy> GetDeliveryPoliciesByDate(DateTime date)
        {
            return _dbContext.DeliveryPolicies
                .Where(dp => dp.EffectiveDate <= date)
                .OrderByDescending(dp => dp.EffectiveDate)
                .ToList();
        }
    }
}
