using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Manager;
using ASI.Basecode.Services.ServiceModels;
using AutoMapper;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Services.Services
{
    public class RestaurantProfileService : IRestaurantProfileService
    {
        private readonly IRestaurantProfileRepository _repository;

        public RestaurantProfileService(IRestaurantProfileRepository repository)
        {
            _repository = repository;
        }

        public Restaurant GetRestaurantProfile()
        {
            return _repository.GetRestaurantProfile();
        }

        public void EditRestaurantInformation(Restaurant model)
        {
            // Validate the model
            ValidateRestaurantProfile(model);
            ValidateBusinessHours(model.OpeningTime, model.ClosingTime);
            ValidateContactInformation(model.Email, model.ContactNumber);

            // Update the profile
            _repository.EditRestaurantProfile(model);
        }

        public void EditRestaurantAddress(Address address)
        {
            
        }

        private void ValidateRestaurantProfile(Restaurant model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                throw new ArgumentException("Restaurant name is required.", nameof(model.Name));
            }

            if (model.Name.Length > 100)
            {
                throw new ArgumentException("Restaurant name cannot exceed 100 characters.", nameof(model.Name));
            }

            if (string.IsNullOrWhiteSpace(model.Email))
            {
                throw new ArgumentException("Email is required.", nameof(model.Email));
            }

            if (string.IsNullOrWhiteSpace(model.ContactNumber))
            {
                throw new ArgumentException("Phone number is required.", nameof(model.ContactNumber));
            }
        }

        private static void ValidateBusinessHours(TimeOnly openingTime, TimeOnly closingTime)
        {
            if (openingTime >= closingTime)
            {
                throw new ArgumentException("Opening time must be before closing time.");
            }

            // Check if business hours are within reasonable limits (e.g., 24 hours)
            var duration = closingTime.ToTimeSpan() - openingTime.ToTimeSpan();
            if (duration.TotalHours > 24)
            {
                throw new ArgumentException("Business hours cannot exceed 24 hours.");
            }
        }

        private static void ValidateContactInformation(string email, string contactNumber)
        {
            // Validate email format
            if (!System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                throw new ArgumentException("Invalid email format.", nameof(email));
            }

            // Validate contact number format (basic validation)
            if (!System.Text.RegularExpressions.Regex.IsMatch(contactNumber,
                @"^[\d\s\-\+\(\)]+$"))
            {
                throw new ArgumentException("Invalid contact number format.", nameof(contactNumber));
            }

            if (contactNumber.Length < 7 || contactNumber.Length > 20)
            {
                throw new ArgumentException("Contact number must be between 7 and 20 characters.",
                    nameof(contactNumber));
            }
        }

        private void ValidateAddress(Address address)
        {
            if (string.IsNullOrWhiteSpace(address.Street))
            {
                throw new ArgumentException("Street address is required.", nameof(address.Street));
            }

            if (string.IsNullOrWhiteSpace(address.City))
            {
                throw new ArgumentException("City is required.", nameof(address.City));
            }

            if (string.IsNullOrWhiteSpace(address.Country))
            {
                throw new ArgumentException("Country is required.", nameof(address.Country));
            }

            if (string.IsNullOrWhiteSpace(address.ZipCode.ToString()))
            {
                throw new ArgumentException("Zip code is required.", nameof(address.ZipCode));
            }

            // Validate coordinates if provided
            if (double.TryParse(address.Latitude.ToString(), out double latitude) && (latitude < -90 || latitude > 90))
            {
                throw new ArgumentException("Latitude must be between -90 and 90 degrees.",
                    nameof(address.Latitude));
            }

            if (double.TryParse(address.Longitude.ToString(), out double longitude) && (longitude < -180 || longitude > 180))
            {
                throw new ArgumentException("Longitude must be between -180 and 180 degrees.",
                    nameof(address.Longitude));
            }
        }
    }
}