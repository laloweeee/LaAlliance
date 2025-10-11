using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Data.Interfaces
{
    /// <summary>
    /// Interface for PaymentLog Repository
    /// </summary>
    public interface IPaymentLogRepository
    {
        /// <summary>
        /// Add a new payment log
        /// </summary>
        /// <param name="paymentLog"></param>
        void AddPaymentLog(PaymentLog paymentLog);

        /// <summary>
        /// Update an existing payment log
        /// </summary>
        /// <param name="paymentLog"></param>
        void UpdatePaymentLog(PaymentLog paymentLog);

        /// <summary>
        /// Delete a payment log by its ID
        /// </summary>
        /// <param name="paymentLogID"></param>
        void DeletePaymentLog(int paymentLogID);

        /// <summary>
        /// Get a payment log by its ID
        /// </summary>
        /// <param name="paymentLogID"></param>
        /// <returns></returns>
        PaymentLog GetPaymentLogById(int paymentLogID);

        /// <summary>
        /// Get all payment logs for a specific order
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        IEnumerable<PaymentLog> GetPaymentLogsByOrderId(int orderID);

        /// <summary>
        /// Get all payment logs for a specific user
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        IEnumerable<PaymentLog> GetPaymentLogsByUserId(int userID);

        /// <summary>
        /// Get all payment logs
        /// </summary>
        /// <returns></returns>
        IEnumerable<PaymentLog> GetAllPaymentLogs();

        /// <summary>
        /// Get payment logs by status
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        IEnumerable<PaymentLog> GetPaymentLogsByStatus(PaymentStatus status);

        /// <summary>
        /// Get payment logs by transaction reference
        /// </summary>
        /// <param name="transactionReference"></param>
        /// <returns></returns>
        PaymentLog GetPaymentLogByTransactionReference(string transactionReference);
    }
}
