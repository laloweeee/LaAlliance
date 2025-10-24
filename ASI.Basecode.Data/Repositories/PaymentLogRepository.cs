using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Data.Repositories
{
    /// <summary>
    /// Repository for managing PaymentLog entities
    /// </summary>
    public class PaymentLogRepository : BaseRepository, IPaymentLogRepository
    {
        private readonly AsiBasecodeDBContext _dbContext;

        public PaymentLogRepository(IUnitOfWork unitOfWork, AsiBasecodeDBContext dbContext) : base(unitOfWork)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Add a new payment log
        /// </summary>
        /// <param name="paymentLog"></param>
        public void AddPaymentLog(PaymentLog paymentLog)
        {
            _dbContext.PaymentLogs.Add(paymentLog);
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Update an existing payment log
        /// </summary>
        /// <param name="paymentLog"></param>
        public void UpdatePaymentLog(PaymentLog paymentLog)
        {
            _dbContext.PaymentLogs.Update(paymentLog);
            UnitOfWork.SaveChanges();
        }

        /// <summary>
        /// Delete a payment log by its ID
        /// </summary>
        /// <param name="paymentLogID"></param>
        public void DeletePaymentLog(int paymentLogID)
        {
            var paymentLog = _dbContext.PaymentLogs.Find(paymentLogID);
            if (paymentLog != null)
            {
                _dbContext.PaymentLogs.Remove(paymentLog);
                UnitOfWork.SaveChanges();
            }
        }

        /// <summary>
        /// Get a payment log by its ID
        /// </summary>
        /// <param name="paymentLogID"></param>
        /// <returns></returns>
        public PaymentLog GetPaymentLogById(int paymentLogID)
        {
            return _dbContext.PaymentLogs
                .Include(pl => pl.Order)
                .Include(pl => pl.User)
                .FirstOrDefault(pl => pl.PaymentLogID == paymentLogID);
        }

        /// <summary>
        /// Get all payment logs for a specific order
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public IQueryable<PaymentLog> GetPaymentLogsByOrderId(int orderID)
        {
            return _dbContext.PaymentLogs.Where(pl => pl.OrderID == orderID).AsQueryable();
        }

        /// <summary>
        /// Get all payment logs for a specific user
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public IEnumerable<PaymentLog> GetPaymentLogsByUserId(int userID)
        {
            return _dbContext.PaymentLogs
                .Include(pl => pl.Order)
                .Where(pl => pl.UserID == userID)
                .OrderByDescending(pl => pl.PaymentDate)
                .ToList();
        }

        /// <summary>
        /// Get all payment logs
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PaymentLog> GetAllPaymentLogs()
        {
            return _dbContext.PaymentLogs
                .Include(pl => pl.Order)
                .Include(pl => pl.User)
                .OrderByDescending(pl => pl.PaymentDate)
                .ToList();
        }

        /// <summary>
        /// Get payment logs by status
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public IEnumerable<PaymentLog> GetPaymentLogsByStatus(PaymentStatus status)
        {
            return _dbContext.PaymentLogs
                .Include(pl => pl.Order)
                .Include(pl => pl.User)
                .Where(pl => pl.PaymentStatus == status)
                .OrderByDescending(pl => pl.PaymentDate)
                .ToList();
        }

        /// <summary>
        /// Get payment logs by transaction reference
        /// </summary>
        /// <param name="transactionReference"></param>
        /// <returns></returns>
        public PaymentLog GetPaymentLogByTransactionReference(string transactionReference)
        {
            return _dbContext.PaymentLogs
                .Include(pl => pl.Order)
                .Include(pl => pl.User)
                .FirstOrDefault(pl => pl.TransactionReference == transactionReference);
        }
    }
}
