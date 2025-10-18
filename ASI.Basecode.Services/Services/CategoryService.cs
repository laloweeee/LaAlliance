using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Manager;
using ASI.Basecode.Services.ServiceModels;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IProductRepository _productRepository;

        public CategoryService(ICategoryRepository categoryRepository, ILoggerFactory loggerFactory, IProductRepository productRepository)
        {
            _categoryRepository = categoryRepository;
            _loggerFactory = loggerFactory;
            _productRepository = productRepository;
        }

        /// <summary>
        /// Get all categories
        /// </summary>
        /// <returns></returns>
        public List<ProductCategory> GetAllCategories()
        {
            return _categoryRepository
                .GetAllIncludingDeleted<ProductCategory>()
                .OrderBy(c => c.IsDeleted)
                .ThenBy(c => c.CategoryName)
                .ToList();
        }

        /// <summary>
        /// Get category by ID
        /// </summary>
        /// <param name="categoryID"></param>
        /// <returns></returns>
        public ProductCategory GetCategoryByID(int categoryID)
        {
            return _categoryRepository
                .GetAllIncludingDeleted<ProductCategory>()
                .FirstOrDefault(c => c.CategoryID == categoryID);
        }

        /// <summary>
        /// Add a new category
        /// </summary>
        /// <param name="model"></param>
        /// <exception cref="ArgumentException"></exception>
        public void AddCategory(CategoryViewModel model)
        {
            var existingCategory = _categoryRepository.GetAllCategories()
                .FirstOrDefault(c => c.CategoryName.ToLower().Trim() == model.CategoryName.ToLower().Trim());

            if (existingCategory != null)
            {
                throw new ArgumentException("A category with this name already exists.");
            }

            var category = new ProductCategory
            {
                CategoryName = model.CategoryName.Trim(),
                IsActive = model.IsActive,
            };

            _categoryRepository.AddCategory(category);
        }

        /// <summary>
        /// Update an existing category
        /// </summary>
        /// <param name="model"></param>
        /// <exception cref="ArgumentException"></exception>
        public void UpdateCategory(CategoryViewModel model)
        {
            var category = _categoryRepository.GetAllCategories().FirstOrDefault(c => c.CategoryID == model.CategoryID) ?? throw new ArgumentException("Category not found.");

            var existingCategory = _categoryRepository.GetAllCategories()
                .FirstOrDefault(c => c.CategoryName.ToLower().Trim() == model.CategoryName.ToLower().Trim() && c.CategoryID != model.CategoryID);

            if (existingCategory != null)
            {
                throw new ArgumentException("A category with this name already exists.");
            }

            category.IsActive = model.IsActive;

            _categoryRepository.UpdateCategory(category);
        }

        /// <summary>
        /// Delete a category
        /// </summary>
        /// <param name="categoryID"></param>
        public void DeleteCategory(int categoryID, string deletedBy)
        {
            var category = _categoryRepository.GetAllCategories().FirstOrDefault(c => c.CategoryID == categoryID);

            if (category == null)
            {
                throw new ArgumentException("Category not found.");
            }

            var productCount = _productRepository.GetProducts()
                .Count(p => p.CategoryID == categoryID);

            if (productCount > 0)
            {
                throw new InvalidOperationException("Cannot delete category with associated products.");
            }

            if (category.IsActive)
            {
                throw new InvalidOperationException("Cannot delete an active category. Please deactivate it first.");
            }

            _categoryRepository.SoftDelete(category, deletedBy);
        }

        /// <summary>
        /// Recover a soft-deleted category
        /// </summary>
        /// <param name="categoryID"></param>
        /// <exception cref="ArgumentException"></exception>
        public void RecoverCategory(int categoryID)
        {
            _loggerFactory.CreateLogger<CategoryService>().LogInformation($"Attempting to recover category with ID: {categoryID}");
            
            var category = GetCategoryByID(categoryID);

            if (category == null)
            {
                throw new ArgumentException("Category not found.");
            }

            _categoryRepository.Restore(category);
        }

        /// <summary>
        /// Permanently delete a category
        /// </summary>
        /// <param name="categoryID"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public void PermanentDeleteCategory(int categoryID)
        {
            var category = GetCategoryByID(categoryID);

            if (category == null)
            {
                throw new ArgumentException("Category not found.");
            }

            if (category.Products.Count() > 0)
            {
                throw new InvalidOperationException("Cannot permanently delete category with associated products.");
            }

            _categoryRepository.HardDelete(category);
        }
    }
}
