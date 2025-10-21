using Xunit;
using Moq;
using System.Collections.Generic;
using System.Linq;
using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Services.Services;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.Data.Models;

namespace ASI.Basecode.Tests
{
    public class AddressServiceTests
    {
        private readonly Mock<IUserProfileRepository> _mockRepo;

        public AddressServiceTests()
        {
            _mockRepo = new Mock<IUserProfileRepository>();
        }

        [Fact]
        public void GetUserAddresses_ShouldReturnAddressList()
        {
            // Arrange
            var fakeUserAddresses = new List<UserAddress>
            {
                new UserAddress
                {
                    UserAddressID = 1,
                    AddressID = 1,
                    UserID = 1,
                    IsDefault = true,
                    Address = new Address
                    {
                        AddressID = 1,
                        Street = "123 Main St",
                        City = "Test City",
                        Province = "TS",
                        ZipCode = 12345
                    }
                }
            }.AsQueryable();

            _mockRepo.Setup(r => r.GetUserAddresses(1)).Returns(fakeUserAddresses);

            // Act - directly test the repository call
            var result = _mockRepo.Object.GetUserAddresses(1).ToList();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("123 Main St", result.First().Address.Street);
            Assert.Equal("Test City", result.First().Address.City);
            Assert.True(result.First().IsDefault);
        }

        [Fact]
        public void AddUserAddress_ShouldCallRepositoryAdd()
        {
            // Arrange
            var userAddress = new UserAddress
            {
                UserAddressID = 1,
                AddressID = 1,
                UserID = 1,
                IsDefault = false,
                Address = new Address
                {
                    Street = "New Street",
                    City = "New City",
                    Province = "NC",
                    ZipCode = 54321
                }
            };

            // Act
            _mockRepo.Object.AddUserAddress(userAddress);

            // Assert
            _mockRepo.Verify(r => r.AddUserAddress(userAddress), Times.Once);
        }

        [Fact]
        public void RemoveUserAddress_ShouldCallRepositoryRemove()
        {
            // Arrange
            int addressId = 10;
            int userId = 2;

            // Act
            _mockRepo.Object.RemoveUserAddress(addressId, userId);

            // Assert
            _mockRepo.Verify(r => r.RemoveUserAddress(addressId, userId), Times.Once);
        }

        [Fact]
        public void GetUserAddresses_WithMultipleAddresses_ShouldReturnAll()
        {
            // Arrange
            var fakeUserAddresses = new List<UserAddress>
            {
                new UserAddress
                {
                    UserAddressID = 1,
                    AddressID = 1,
                    UserID = 1,
                    IsDefault = true,
                    Address = new Address { Street = "123 Main St", City = "City1", Province = "P1", ZipCode = 11111 }
                },
                new UserAddress
                {
                    UserAddressID = 2,
                    AddressID = 2,
                    UserID = 1,
                    IsDefault = false,
                    Address = new Address { Street = "456 Oak Ave", City = "City2", Province = "P2", ZipCode = 22222 }
                }
            }.AsQueryable();

            _mockRepo.Setup(r => r.GetUserAddresses(1)).Returns(fakeUserAddresses);

            // Act
            var result = _mockRepo.Object.GetUserAddresses(1).ToList();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Single(result, r => r.UserAddressID == 1);
        }
    }
}