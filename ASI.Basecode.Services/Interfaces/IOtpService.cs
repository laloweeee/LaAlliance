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
        public void MarkVerified(string email);
    }
}
