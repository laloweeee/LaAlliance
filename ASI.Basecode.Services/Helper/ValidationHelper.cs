using System;
using System.Text.RegularExpressions;

namespace ASI.Basecode.Services.Helper
{
    public static class ValidationHelper
    {
        public static void ValidateNotNull(object obj, string paramName)
        {
            if (obj == null)
                throw new ArgumentNullException(paramName);
        }

        public static void ValidateRequired(string value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{fieldName} is required.");
        }

        public static void ValidatePositiveInt(int value, string fieldName)
        {
            if (value <= 0)
                throw new ArgumentException($"Invalid {fieldName}.");
        }

        public static void ValidateEmail(string email)
        {
            if (!string.IsNullOrWhiteSpace(email) &&
                !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new ArgumentException("Invalid email format.");
        }
    }
}