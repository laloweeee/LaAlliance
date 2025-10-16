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


        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        /// <summary>
        /// Get all categories
        /// </summary>
        /// <returns></returns>
        public List<ProductCategory> GetAllCategories()
        {
            return _categoryRepository.GetAllCategories().OrderBy(c => c.CategoryName).ToList();
        }

        /// <summary>
        /// Get category by ID
        /// </summary>
        /// <param name="categoryID"></param>
        /// <returns></returns>
        public ProductCategory GetCategoryByID(int categoryID)
        {
            return _categoryRepository.GetAllCategories().FirstOrDefault(c => c.CategoryID == categoryID);
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
        public void DeleteCategory(int categoryID)
        {
            _categoryRepository.DeleteCategory(categoryID);
        }
    }
}
