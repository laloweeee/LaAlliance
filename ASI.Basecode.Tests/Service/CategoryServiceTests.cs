using Xunit;
using Moq;
using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.Services.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASI.Basecode.Tests.Service
{
    public class CategoryServiceTests
    {
        private readonly Mock<ICategoryRepository> _categoryRepoMock;
        private readonly Mock<IProductRepository> _productRepoMock;
        private readonly Mock<ILoggerFactory> _loggerFactoryMock;
        private readonly CategoryService _service;

        public CategoryServiceTests()
        {
            _categoryRepoMock = new Mock<ICategoryRepository>();
            _productRepoMock = new Mock<IProductRepository>();
            _loggerFactoryMock = new Mock<ILoggerFactory>();

            // Add this block to mock the logger
            var loggerMock = new Mock<ILogger<CategoryService>>();
            _loggerFactoryMock
                .Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(loggerMock.Object);

            _service = new CategoryService(_categoryRepoMock.Object, _loggerFactoryMock.Object, _productRepoMock.Object);
        }

        [Fact]
        public void GetAllCategories_ReturnsOrderedList()
        {
            var categories = new List<ProductCategory>
            {
                new ProductCategory { CategoryID = 1, CategoryName = "B", IsDeleted = false },
                new ProductCategory { CategoryID = 2, CategoryName = "A", IsDeleted = true }
            };
            _categoryRepoMock.Setup(r => r.GetAllIncludingDeleted<ProductCategory>()).Returns(categories.AsQueryable());

            var result = _service.GetAllCategories();

            Assert.Equal(2, result.Count);
            Assert.Equal("B", result[0].CategoryName);
            Assert.Equal("A", result[1].CategoryName);
        }

        [Fact]
        public void GetCategoryByID_ReturnsCategory()
        {
            var categories = new List<ProductCategory>
            {
                new ProductCategory { CategoryID = 1, CategoryName = "Test" }
            };
            _categoryRepoMock.Setup(r => r.GetAllIncludingDeleted<ProductCategory>()).Returns(categories.AsQueryable());

            var result = _service.GetCategoryByID(1);

            Assert.NotNull(result);
            Assert.Equal("Test", result.CategoryName);
        }

        [Fact]
        public void GetDeletedProductCategories_ReturnsOnlyDeleted()
        {
            var categories = new List<ProductCategory>
            {
                new ProductCategory { CategoryID = 1, CategoryName = "A", IsDeleted = true },
                new ProductCategory { CategoryID = 2, CategoryName = "B", IsDeleted = false }
            };
            _categoryRepoMock.Setup(r => r.GetAllIncludingDeleted<ProductCategory>()).Returns(categories.AsQueryable());

            var result = _service.GetDeletedProductCategories();

            Assert.Single(result);
            Assert.Equal("A", result[0].CategoryName);
        }

        [Fact]
        public void AddCategory_ThrowsIfDuplicateName()
        {
            var categories = new List<ProductCategory>
            {
                new ProductCategory { CategoryID = 1, CategoryName = "Test" }
            };
            _categoryRepoMock.Setup(r => r.GetAllCategories()).Returns(categories.AsQueryable());

            var model = new CategoryViewModel { CategoryName = "Test", IsActive = true };

            Assert.Throws<ArgumentException>(() => _service.AddCategory(model));
        }

        [Fact]
        public void AddCategory_AddsIfUnique()
        {
            _categoryRepoMock.Setup(r => r.GetAllCategories()).Returns(new List<ProductCategory>().AsQueryable());
            var model = new CategoryViewModel { CategoryName = "New", IsActive = true };

            _service.AddCategory(model);

            _categoryRepoMock.Verify(r => r.AddCategory(It.Is<ProductCategory>(c => c.CategoryName == "New" && c.IsActive)), Times.Once);
        }

        [Fact]
        public void UpdateCategory_ThrowsIfNotFound()
        {
            _categoryRepoMock.Setup(r => r.GetAllCategories()).Returns(new List<ProductCategory>().AsQueryable());
            var model = new CategoryViewModel { CategoryID = 99, CategoryName = "Test", IsActive = true };

            Assert.Throws<ArgumentException>(() => _service.UpdateCategory(model));
        }

        [Fact]
        public void UpdateCategory_ThrowsIfDuplicateName()
        {
            var categories = new List<ProductCategory>
            {
                new ProductCategory { CategoryID = 1, CategoryName = "Test" },
                new ProductCategory { CategoryID = 2, CategoryName = "Other" }
            };
            _categoryRepoMock.Setup(r => r.GetAllCategories()).Returns(categories.AsQueryable());
            var model = new CategoryViewModel { CategoryID = 1, CategoryName = "Other", IsActive = true };

            Assert.Throws<ArgumentException>(() => _service.UpdateCategory(model));
        }

        [Fact]
        public void UpdateCategory_UpdatesIfValid()
        {
            var categories = new List<ProductCategory>
            {
                new ProductCategory { CategoryID = 1, CategoryName = "Test", IsActive = false }
            };
            _categoryRepoMock.Setup(r => r.GetAllCategories()).Returns(categories.AsQueryable());
            var model = new CategoryViewModel { CategoryID = 1, CategoryName = "Test", IsActive = true };

            _service.UpdateCategory(model);

            _categoryRepoMock.Verify(r => r.UpdateCategory(It.Is<ProductCategory>(c => c.CategoryID == 1 && c.IsActive)), Times.Once);
        }

        [Fact]
        public void DeleteCategory_ThrowsIfNotFound()
        {
            _categoryRepoMock.Setup(r => r.GetAllCategories()).Returns(new List<ProductCategory>().AsQueryable());

            Assert.Throws<ArgumentException>(() => _service.DeleteCategory(1, "user"));
        }

        [Fact]
        public void DeleteCategory_ThrowsIfHasProducts()
        {
            var categories = new List<ProductCategory>
            {
                new ProductCategory { CategoryID = 1, CategoryName = "Test", IsActive = false }
            };
            _categoryRepoMock.Setup(r => r.GetAllCategories()).Returns(categories.AsQueryable());
            _productRepoMock.Setup(r => r.GetProducts()).Returns(new List<Product> { new Product { CategoryID = 1 } }.AsQueryable());

            Assert.Throws<InvalidOperationException>(() => _service.DeleteCategory(1, "user"));
        }

        [Fact]
        public void DeleteCategory_ThrowsIfActive()
        {
            var categories = new List<ProductCategory>
            {
                new ProductCategory { CategoryID = 1, CategoryName = "Test", IsActive = true }
            };
            _categoryRepoMock.Setup(r => r.GetAllCategories()).Returns(categories.AsQueryable());
            _productRepoMock.Setup(r => r.GetProducts()).Returns(new List<Product>().AsQueryable());

            Assert.Throws<InvalidOperationException>(() => _service.DeleteCategory(1, "user"));
        }

        [Fact]
        public void DeleteCategory_SoftDeletesIfValid()
        {
            var categories = new List<ProductCategory>
            {
                new ProductCategory { CategoryID = 1, CategoryName = "Test", IsActive = false }
            };
            _categoryRepoMock.Setup(r => r.GetAllCategories()).Returns(categories.AsQueryable());
            _productRepoMock.Setup(r => r.GetProducts()).Returns(new List<Product>().AsQueryable());

            _service.DeleteCategory(1, "user");

            _categoryRepoMock.Verify(r => r.SoftDelete(It.Is<ProductCategory>(c => c.CategoryID == 1), "user"), Times.Once);
        }

        [Fact]
        public void RecoverCategory_ThrowsIfNotFound()
        {
            _categoryRepoMock.Setup(r => r.GetAllIncludingDeleted<ProductCategory>()).Returns(new List<ProductCategory>().AsQueryable());

            Assert.Throws<ArgumentException>(() => _service.RecoverCategory(1));
        }

        [Fact]
        public void RecoverCategory_CallsRestore()
        {
            var categories = new List<ProductCategory>
            {
                new ProductCategory { CategoryID = 1, CategoryName = "Test", IsDeleted = true }
            };
            _categoryRepoMock.Setup(r => r.GetAllIncludingDeleted<ProductCategory>()).Returns(categories.AsQueryable());

            _service.RecoverCategory(1);

            _categoryRepoMock.Verify(r => r.Restore(It.Is<ProductCategory>(c => c.CategoryID == 1)), Times.Once);
        }

        [Fact]
        public void PermanentDeleteCategory_ThrowsIfNotFound()
        {
            _categoryRepoMock.Setup(r => r.GetAllIncludingDeleted<ProductCategory>()).Returns(new List<ProductCategory>().AsQueryable());

            Assert.Throws<ArgumentException>(() => _service.PermanentDeleteCategory(1));
        }

        [Fact]
        public void PermanentDeleteCategory_ThrowsIfHasProducts()
        {
            var categories = new List<ProductCategory>
            {
                new ProductCategory { CategoryID = 1, CategoryName = "Test", Products = new List<Product> { new Product() } }
            };
            _categoryRepoMock.Setup(r => r.GetAllIncludingDeleted<ProductCategory>()).Returns(categories.AsQueryable());

            Assert.Throws<InvalidOperationException>(() => _service.PermanentDeleteCategory(1));
        }

        [Fact]
        public void PermanentDeleteCategory_HardDeletesIfValid()
        {
            var categories = new List<ProductCategory>
            {
                new ProductCategory { CategoryID = 1, CategoryName = "Test", Products = new List<Product>() }
            };
            _categoryRepoMock.Setup(r => r.GetAllIncludingDeleted<ProductCategory>()).Returns(categories.AsQueryable());

            _service.PermanentDeleteCategory(1);

            _categoryRepoMock.Verify(r => r.HardDelete(It.Is<ProductCategory>(c => c.CategoryID == 1)), Times.Once);
        }
    }
}