using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Services
{
    public class OtpService : IOtpService
    {
        private readonly Random _random = new();
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _repository;

        public OtpService(IConfiguration configuration, IUserRepository repository)
        {
            _configuration = configuration;
            _repository = repository;
        }

        public int GenerateOtp()
        {
            return _random.Next(100000, 999999);
        }

        private string GetSecretKey()
        {
            string secretKey = _configuration["TokenAuthentication:SecretKey"];

            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("Secret key is not configured.");
            }

            return secretKey;
        }

        public string HashOtp(int otp, DateTime timestamp)
        {
            string secretKey = GetSecretKey();
            string dataToHash = $"{otp}|{timestamp:yyyyMMddHHmm}|{secretKey}";

            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
            {
                byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(dataToHash));
                return Convert.ToBase64String(hashBytes);
            }
        }

        public bool VerifyOtp(string email, int otpCode)
        {
            var otpDetails = GetOtpDetailsForUser(email);

            if (otpDetails == null)
                return false;

            DateTime timestamp = otpDetails.Timestamp;
            string storedHash = otpDetails.HashedOtp;

            if (DateTime.UtcNow > timestamp.AddMinutes(10))
                return false;

            string computedHash = HashOtp(otpCode, timestamp);

            try
            {
                byte[] hash1 = Convert.FromBase64String(computedHash);
                byte[] hash2 = Convert.FromBase64String(storedHash);
                return hash1.Length == hash2.Length && CryptographicOperations.FixedTimeEquals(hash1, hash2);
            }
            catch
            {
                return computedHash == storedHash;
            }
        }

        /// <summary>
        /// Store OTP for user function
        /// </summary>
        /// <param name="email"></param>
        /// <param name="hashedOtp"></param>
        /// <param name="timestamp"></param>
        /// <exception cref="InvalidDataException"></exception>
        public void StoreOtpForUser(string email, string hashedOtp, DateTime timestamp)
        {
            var user = _repository.GetUsers().FirstOrDefault(u => u.Email == email) ?? throw new InvalidDataException(Resources.Messages.Errors.UserExists);

            user.EmailHashToken = hashedOtp;
            user.EmailTokenExpiry = timestamp;
            _repository.UpdateUser(user);
        }

        /// <summary>
        /// Store OTP for user function
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        /// <exception cref="InvalidDataException"></exception>
        public OtpModel GetOtpDetailsForUser(string email)
        {
            var user = _repository.GetUsers().FirstOrDefault(u => u.Email == email) ?? throw new InvalidDataException(Resources.Messages.Errors.UserExists);

            return new OtpModel
            {
                HashedOtp = user.EmailHashToken,
                Timestamp = user.EmailTokenExpiry.Value
            };
        }

        /// <summary>
        /// Mark user email as verified function
        /// </summary>
        /// <param name="email"></param>
        /// <exception cref="InvalidDataException"></exception>
        public void MarkVerified(string email)
        {
            var user = _repository.GetUsers().First(u => u.Email == email) ?? throw new InvalidDataException(Resources.Messages.Errors.UserExists);

            user.IsEmailVerified = true;
            user.EmailHashToken = null;
            user.EmailTokenExpiry = null;

            _repository.UpdateUser(user);
        }
    }
}
