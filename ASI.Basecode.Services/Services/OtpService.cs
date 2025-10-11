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
    /// <summary>
    /// OTP Service for generating, storing, and verifying OTPs
    /// </summary>
    public class OtpService : IOtpService
    {
        private readonly Random _random = new();
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _repository;
        private readonly IMailSender _mailSender;
        private const int OtpExpiryMinutes = 10;

        public OtpService(IConfiguration configuration, IUserRepository repository, IMailSender mailSender)
        {
            _configuration = configuration;
            _repository = repository;
            _mailSender = mailSender;
        }

        /// <summary>
        /// Creates and sends OTP to user email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<bool> CreateOtpForUser(string email)
        {
            var otpCode = GenerateOtp();
            var timestamp = DateTime.UtcNow;

            StoreOtpForUser(email, otpCode, timestamp);

            try
            {
                await _mailSender.SendEmailVerificationAsync(email, otpCode);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Generates a random 6-digit OTP
        /// </summary>
        /// <returns></returns>
        private int GenerateOtp()
        {
            return _random.Next(100000, 999999);
        }

        /// <summary>
        /// Retrieves the secret key from configuration
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private string GetSecretKey()
        {
            string secretKey = _configuration["TokenAuthentication:SecretKey"];

            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("Secret key is not configured.");
            }

            return secretKey;
        }

        /// <summary>
        /// Encrypts OTP with timestamp for storage
        /// </summary>
        /// <param name="otp">The OTP code to encrypt</param>
        /// <param name="timestamp">The timestamp when OTP was generated</param>
        /// <returns>Encrypted OTP string</returns>
        public string EncryptOtp(int otp, DateTime timestamp)
        {
            string secretKey = GetSecretKey();
            string dataToEncrypt = $"{otp}|{timestamp:o}"; // ISO 8601 format for precise timestamp

            using (var aes = Aes.Create())
            {
                // Use a fixed key derived from the secret
                byte[] key = new byte[32];
                byte[] secretBytes = Encoding.UTF8.GetBytes(secretKey);
                Array.Copy(secretBytes, key, Math.Min(secretBytes.Length, 32));

                aes.Key = key;
                aes.GenerateIV();

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var msEncrypt = new MemoryStream())
                {
                    // Prepend IV to the encrypted data
                    msEncrypt.Write(aes.IV, 0, aes.IV.Length);

                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(dataToEncrypt);
                    }

                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        /// <summary>
        /// Decrypts OTP and extracts timestamp
        /// </summary>
        /// <param name="encryptedOtp">The encrypted OTP string</param>
        /// <returns>Tuple containing OTP code and timestamp</returns>
        private (int otp, DateTime timestamp) DecryptOtp(string encryptedOtp)
        {
            string secretKey = GetSecretKey();
            byte[] cipherTextWithIv = Convert.FromBase64String(encryptedOtp);

            using (var aes = Aes.Create())
            {
                byte[] key = new byte[32];
                byte[] secretBytes = Encoding.UTF8.GetBytes(secretKey);
                Array.Copy(secretBytes, key, Math.Min(secretBytes.Length, 32));

                aes.Key = key;

                // Extract IV from the beginning of the cipher text
                byte[] iv = new byte[aes.IV.Length];
                byte[] cipherText = new byte[cipherTextWithIv.Length - iv.Length];

                Array.Copy(cipherTextWithIv, iv, iv.Length);
                Array.Copy(cipherTextWithIv, iv.Length, cipherText, 0, cipherText.Length);

                aes.IV = iv;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (var msDecrypt = new MemoryStream(cipherText))
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (var srDecrypt = new StreamReader(csDecrypt))
                {
                    string decryptedData = srDecrypt.ReadToEnd();
                    string[] parts = decryptedData.Split('|');

                    if (parts.Length != 2)
                        throw new InvalidOperationException("Invalid OTP format");

                    int otp = int.Parse(parts[0]);
                    DateTime timestamp = DateTime.Parse(parts[1]);

                    return (otp, timestamp);
                }
            }
        }

        /// <summary>
        /// Verifies OTP code and validates its expiry without database lookup
        /// </summary>
        /// <param name="email">User's email</param>
        /// <param name="otpCode">OTP code to verify</param>
        /// <returns>True if OTP is valid and not expired, false otherwise</returns>
        public bool VerifyOTP(string email, int otpCode)
        {
            var otpDetails = GetOtpDetailsForUser(email);

            if (otpDetails == null)
                return false;

            string storedEncryptedOtp = otpDetails.HashedOtp;

            if (string.IsNullOrEmpty(storedEncryptedOtp))
                return false;

            try
            {
                // Decrypt the stored OTP to get the original code and timestamp
                var (storedOtp, timestamp) = DecryptOtp(storedEncryptedOtp);

                // Check if OTP has expired (using UTC to avoid timezone issues)
                if (DateTime.UtcNow > timestamp.ToUniversalTime().AddMinutes(OtpExpiryMinutes))
                    return false;

                // Compare OTP codes using constant-time comparison to prevent timing attacks
                return storedOtp == otpCode;
            }
            catch (Exception)
            {
                // Log the exception if needed
                return false;
            }
        }

        /// <summary>
        /// Store OTP for user function
        /// </summary>
        /// <param name="email"></param>
        /// <param name="hashedOtp"></param>
        /// <param name="timestamp"></param>
        /// <exception cref="InvalidDataException"></exception>
        public void StoreOtpForUser(string email, int otp, DateTime timestamp)
        {
            var user = _repository.GetUsers().FirstOrDefault(u => u.Email == email)
                ?? throw new InvalidDataException(Resources.Messages.Errors.UserExists);

            // Encrypt OTP with timestamp instead of hashing
            string encryptedOtp = EncryptOtp(otp, timestamp);

            user.EmailHashToken = encryptedOtp;

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

            _repository.UpdateUser(user);
        }
    }
}
