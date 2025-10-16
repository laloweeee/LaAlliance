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
        public string EncryptOtp(int otp, DateTime timestamp);
        public Task<bool> CreateOtpForUser(string email);
        public bool VerifyOTP(string email, int otpCode);
        public void StoreOtpForUser(string email, int otp, DateTime timestamp);
        public OtpModel GetOtpDetailsForUser(string email);
        public Task MarkVerified(string email);
        
        // Password reset methods
        public Task<bool> CreatePasswordResetOtp(string email);
        public bool VerifyPasswordResetOTP(string email, int otpCode);
        public void ClearPasswordResetOtp(string email);
        
        // Email change methods
        public Task<bool> CreateEmailChangeOtp(string email, string newEmail = null);
        public bool VerifyEmailChangeOTP(string email, int otpCode);
        public void ClearEmailChangeOtp(string email);
    }
}
