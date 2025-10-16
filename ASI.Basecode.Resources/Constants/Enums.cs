namespace ASI.Basecode.Resources.Constants
{
    /// <summary>
    /// Class for enumerated values
    /// </summary>
    public class Enums
    {
        /// <summary>
        /// API Result Status
        /// </summary>
        public enum Status
        {
            Success,
            Error,
            CustomErr,
        }

        /// <summary>
        /// Login Result
        /// </summary>
        public enum LoginResult
        {
            Success = 0,
            Failed = 1,
        }

        /// <summary>
        /// User Type
        /// </summary>
        public enum UserType
        {
            Customer,
            Restaurant
        }

        /// <summary>
        /// Order Status
        /// </summary>
        public enum OrderStatus
        {
            Pending,
            Processing,
            Completed,
            Cancelled,
            Refunded,
            Ready
        }
        /// <summary>
        /// Order Type
        /// </summary>
        public enum OrderType
        {
            Delivery,
            Pickup
        }
        /// <summary>
        /// Payment Method
        /// </summary>
        public enum PaymentMethod
        {
            CreditCard,
            CashOnDelivery
        }
        /// <summary>
        /// Payment Status
        /// </summary>
        public enum PaymentStatus
        {
            Pending,
            Completed,
            Failed,
            Refunded
        }
        /// <summary>
        /// Discount Type
        /// </summary>
        public enum DiscountType
        {
            Percentage,
            FixedAmount
        }
        /// <summary>
        /// Staff Role
        /// </summary>
        public enum StaffRole
        {
            Admin,
            Staff
        }

        /// <summary>
        /// Account Status
        /// </summary>
        public enum AccountStatus
        {
            Active,
            Inactive,
            Disabled
        }
    }
}
