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
        /// User Role
        /// </summary>
        public enum UserRole
        {
            Customer = 0,
            Restaurant = 1
        }

        /// <summary>
        /// Restaurant Permission
        /// </summary>
        public enum RestaurantPermission
        {
            Admin = 0,
            Staff = 1,
        }
    }
}
