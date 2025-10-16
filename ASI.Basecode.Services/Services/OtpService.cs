using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
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
        private readonly IUserRepository _userRepository;
        private readonly IEmailVerificationTokenRepository _emailTokenRepository;
        private readonly IPasswordResetTokenRepository _passwordResetTokenRepository;
        private readonly IMailSender _mailSender;
        private const int OtpExpiryMinutes = 10;

        public OtpService(
            IConfiguration configuration, 
            IUserRepository userRepository, 
            IEmailVerificationTokenRepository emailTokenRepository,
            IPasswordResetTokenRepository passwordResetTokenRepository,
            IMailSender mailSender)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _emailTokenRepository = emailTokenRepository;
            _passwordResetTokenRepository = passwordResetTokenRepository;
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
        /// <param name="otp"></param>
        /// <param name="timestamp"></param>
        /// <exception cref="InvalidDataException"></exception>
        public async void StoreOtpForUser(string email, int otp, DateTime timestamp)
        {
            var user = _userRepository.GetUsers().FirstOrDefault(u => u.Email == email)
                ?? throw new InvalidDataException(Resources.Messages.Errors.UserExists);

            // Encrypt OTP with timestamp
            string encryptedOtp = EncryptOtp(otp, timestamp);

            // Create new email verification token
            var token = new Data.Models.EmailVerificationToken
            {
                UserID = user.UserID,
                Token = encryptedOtp,
                CreatedAt = timestamp,
                ExpiresAt = timestamp.AddMinutes(OtpExpiryMinutes),
                IsUsed = false
            };

            await _emailTokenRepository.CreateAsync(token);
        }

        /// <summary>
        /// Get OTP details for user function
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        /// <exception cref="InvalidDataException"></exception>
        public OtpModel GetOtpDetailsForUser(string email)
        {
            var user = _userRepository.GetUsers().FirstOrDefault(u => u.Email == email) 
                ?? throw new InvalidDataException(Resources.Messages.Errors.UserExists);

            var latestToken = _emailTokenRepository.GetLatestTokenByUserId(user.UserID);

            return new OtpModel
            {
                HashedOtp = latestToken?.Token,
            };
        }

        /// <summary>
        /// Mark user email as verified function
        /// </summary>
        /// <param name="email"></param>
        /// <exception cref="InvalidDataException"></exception>
        public async Task MarkVerified(string email)
        {
            var user = _userRepository.GetUsers().First(u => u.Email == email) 
                ?? throw new InvalidDataException(Resources.Messages.Errors.UserExists);

            user.IsEmailVerified = true;
            _userRepository.UpdateUser(user);

            // Mark the latest token as used
            var latestToken = _emailTokenRepository.GetLatestTokenByUserId(user.UserID);
            if (latestToken != null)
            {
                await _emailTokenRepository.MarkAsUsedAsync(latestToken.TokenID);
            }
        }

        #region Password Reset Methods

        /// <summary>
        /// Creates and sends OTP for password reset
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<bool> CreatePasswordResetOtp(string email)
        {
            var user = _userRepository.GetUsers().FirstOrDefault(x => x.Email == email);

            if (user == null)
            {
                return false;
            }

            var otp = GenerateOtp();
            var timestamp = DateTime.UtcNow;
            var encryptedOtp = EncryptOtp(otp, timestamp);

            // Create new password reset token
            var token = new Data.Models.PasswordResetToken
            {
                UserID = user.UserID,
                Token = encryptedOtp,
                CreatedAt = timestamp,
                ExpiresAt = timestamp.AddMinutes(OtpExpiryMinutes),
                IsUsed = false
            };

            await _passwordResetTokenRepository.CreateAsync(token);
            await _mailSender.SendPasswordResetEmailAsync(email, otp.ToString());
            return true;
        }

        /// <summary>
        /// Verifies password reset OTP
        /// </summary>
        /// <param name="email"></param>
        /// <param name="otpCode"></param>
        /// <returns></returns>
        public bool VerifyPasswordResetOTP(string email, int otpCode)
        {
            var user = _userRepository.GetUsers().FirstOrDefault(x => x.Email == email);

            if (user == null)
                return false;

            var latestToken = _passwordResetTokenRepository.GetLatestTokenByUserId(user.UserID).Result;

            if (latestToken == null)
                return false;

            try
            {
                var (storedOtp, timestamp) = DecryptOtp(latestToken.Token);

                if (storedOtp != otpCode)
                    return false;

                if (DateTime.UtcNow > latestToken.ExpiresAt)
                    return false;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Clears password reset OTP
        /// </summary>
        /// <param name="email"></param>
        public async void ClearPasswordResetOtp(string email)
        {
            var user = _userRepository.GetUsers().FirstOrDefault(x => x.Email == email);

            if (user != null)
            {
                var latestToken = await _passwordResetTokenRepository.GetLatestTokenByUserId(user.UserID);
                if (latestToken != null)
                {
                    await _passwordResetTokenRepository.MarkAsUsedAsync(latestToken.TokenID);
                }
            }
        }

        #endregion

        #region Email Change Methods

        /// <summary>
        /// Creates and sends OTP for email change (can be for current email or new email)
        /// </summary>
        /// <param name="email">Email address to send OTP to</param>
        /// <param name="newEmail">Optional: If provided, stores this as the pending new email</param>
        /// <returns></returns>
        public async Task<bool> CreateEmailChangeOtp(string email, string newEmail = null)
        {
            var user = _userRepository.GetUsers().FirstOrDefault(x => x.Email == email);

            if (user == null)
            {
                return false;
            }

            var otp = GenerateOtp();
            var timestamp = DateTime.UtcNow;
            var encryptedOtp = EncryptOtp(otp, timestamp);

            // Create new email verification token for email change
            var token = new Data.Models.EmailVerificationToken
            {
                UserID = user.UserID,
                Token = encryptedOtp,
                CreatedAt = timestamp,
                ExpiresAt = timestamp.AddMinutes(OtpExpiryMinutes),
                IsUsed = false
            };

            await _emailTokenRepository.CreateAsync(token);
            await _mailSender.SendEmailVerificationAsync(email, otp);
            return true;
        }

        /// <summary>
        /// Verifies email change OTP
        /// </summary>
        /// <param name="email"></param>
        /// <param name="otpCode"></param>
        /// <returns></returns>
        public bool VerifyEmailChangeOTP(string email, int otpCode)
        {
            var user = _userRepository.GetUsers().FirstOrDefault(x => x.Email == email);

            if (user == null)
                return false;

            var latestToken = _emailTokenRepository.GetLatestTokenByUserId(user.UserID);

            if (latestToken == null || string.IsNullOrEmpty(latestToken.Token))
                return false;

            try
            {
                var (storedOtp, timestamp) = DecryptOtp(latestToken.Token);

                if (storedOtp != otpCode)
                    return false;

                if (DateTime.UtcNow > latestToken.ExpiresAt)
                    return false;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Clears email change OTP
        /// </summary>
        /// <param name="email"></param>
        public async void ClearEmailChangeOtp(string email)
        {
            var user = _userRepository.GetUsers().FirstOrDefault(x => x.Email == email);

            if (user != null)
            {
                var latestToken = _emailTokenRepository.GetLatestTokenByUserId(user.UserID);
                if (latestToken != null)
                {
                    await _emailTokenRepository.MarkAsUsedAsync(latestToken.TokenID);
                }
            }
        }

        #endregion
    }
}
