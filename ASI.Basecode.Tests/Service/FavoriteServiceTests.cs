using Xunit;
using Moq;
using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASI.Basecode.Tests.Service
{
    public class FavoriteServiceTests
    {
        private readonly Mock<IFavoriteRepository> _favoriteRepoMock;
        private readonly FavoriteService _service;

        public FavoriteServiceTests()
        {
            _favoriteRepoMock = new Mock<IFavoriteRepository>();
            _service = new FavoriteService(_favoriteRepoMock.Object);
        }

        [Fact]
        public void GetFavoritesByUser_ReturnsFavorites()
        {
            var favorites = new List<CustomerProductFavorite>
            {
                new CustomerProductFavorite { FavoriteID = 1, UserID = 2, ProductID = 10 },
                new CustomerProductFavorite { FavoriteID = 2, UserID = 2, ProductID = 11 }
            };
            _favoriteRepoMock.Setup(r => r.GetFavoritesByUserID(2)).Returns(favorites.AsQueryable());

            var result = _service.GetFavoritesByUser(2);

            Assert.Equal(2, result.Count);
            Assert.Contains(result, f => f.ProductID == 10);
            Assert.Contains(result, f => f.ProductID == 11);
        }

        [Fact]
        public void AddFavorite_AddsIfNotExists()
        {
            var favorites = new List<CustomerProductFavorite>();
            _favoriteRepoMock.Setup(r => r.GetFavoritesByUserID(2)).Returns(favorites.AsQueryable());

            _service.AddFavorite(2, 10);

            _favoriteRepoMock.Verify(r => r.AddFavorite(It.Is<CustomerProductFavorite>(f => f.UserID == 2 && f.ProductID == 10)), Times.Once);
        }

        [Fact]
        public void AddFavorite_DoesNotAddIfAlreadyExists()
        {
            var favorites = new List<CustomerProductFavorite>
            {
                new CustomerProductFavorite { FavoriteID = 1, UserID = 2, ProductID = 10 }
            };
            _favoriteRepoMock.Setup(r => r.GetFavoritesByUserID(2)).Returns(favorites.AsQueryable());

            _service.AddFavorite(2, 10);

            _favoriteRepoMock.Verify(r => r.AddFavorite(It.IsAny<CustomerProductFavorite>()), Times.Never);
        }

        [Fact]
        public void RemoveFavorite_RemovesIfExists()
        {
            var favorite = new CustomerProductFavorite { FavoriteID = 1, UserID = 2, ProductID = 10 };
            _favoriteRepoMock.Setup(r => r.GetFavoriteByID(1)).Returns(favorite);

            _service.RemoveFavorite(1);

            _favoriteRepoMock.Verify(r => r.RemoveFavorite(favorite), Times.Once);
        }

        [Fact]
        public void RemoveFavorite_ThrowsIfNotFound()
        {
            _favoriteRepoMock.Setup(r => r.GetFavoriteByID(99)).Returns((CustomerProductFavorite)null);

            Assert.Throws<ArgumentException>(() => _service.RemoveFavorite(99));
        }
    }
}