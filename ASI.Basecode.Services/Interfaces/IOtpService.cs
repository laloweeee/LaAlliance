using ASI.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IOtpService
    {
        /// <summary>
        /// Generates A new OTP
        /// </summary>
        /// <returns>A new OTP code</returns>
        public int GenerateOtp();

        /// <summary>
        /// Uses TOTP
        /// </summary>
        /// <param name="otp"></param>
        /// <param name="timestamp"></param>
        /// <param name="secretKey"></param>
        /// <returns></returns>
        public string HashOtp(int otp, DateTime timestamp);

        /// <summary>
        /// Verifies the provided OTP against the stored hash and timestamp.
        /// </summary>
        /// <param name="provideOtp"></param>
        /// <param name="storedHash"></param>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public bool VerifyOtp(string email, int otpCode);

        /// <summary>
        /// Store OTP for user function
        /// </summary>
        /// <param name="email"></param>
        /// <param name="hashedOtp"></param>
        /// <param name="timestamp"></param>
        public void StoreOtpForUser(string email, string hashedOtp, DateTime timestamp);

        /// <summary>
        /// Retrieve OTP details for a user function
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public OtpModel GetOtpDetailsForUser(string email);

        /// <summary>
        /// Mark the email as verified function
        /// </summary>
        /// <param name="email"></param>
        public void MarkVerified(string email);
    }
}
