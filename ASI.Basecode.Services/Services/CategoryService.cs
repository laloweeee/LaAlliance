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
        private readonly IUserRepository _userRepository;


        public CategoryService(ICategoryRepository categoryRepository, IUserRepository userRepository)
        {
            _categoryRepository = categoryRepository;
            _userRepository = userRepository;
        }

        public IQueryable<Category> GetAllCategories()
        {
            return _categoryRepository.GetAllCategories();
        }

        public Category GetCategoryByID(int categoryID)
        {
            return _categoryRepository.GetCategoryByID(categoryID);
        }

        public void AddCategory(CategoryViewModel model, int userID)
        {
            var existingCategory = _categoryRepository.GetAllCategories()
                .FirstOrDefault(c => c.Name.ToLower().Trim() == model.Name.ToLower().Trim());

            if (existingCategory != null)
            {
                throw new ArgumentException("A category with this name already exists.");
            }

            var user = _userRepository.GetUserById(userID).FirstOrDefault() ?? throw new ArgumentException("Invalid CreatedBy user ID.");

            var category = new Category
            {
                Name = model.Name.Trim(),
                IsActive = model.IsActive,
                CreatedByUser = user,
                UpdatedByUser = user,
            };

            _categoryRepository.AddCategory(category);
        }

        public void UpdateCategory(CategoryViewModel model, int userID)
        {
            var category = _categoryRepository.GetCategoryByID(model.CategoryID) ?? throw new ArgumentException("Category not found.");

            var existingCategory = _categoryRepository.GetAllCategories()
                .FirstOrDefault(c => c.Name.ToLower().Trim() == model.Name.ToLower().Trim() && c.CategoryID != model.CategoryID);

            if (existingCategory != null)
            {
                throw new ArgumentException("A category with this name already exists.");
            }

            var user = _userRepository.GetUserById(userID).FirstOrDefault() ?? throw new ArgumentException("Invalid UpdatedBy user ID.");

            category.Name = model.Name.Trim();
            category.IsActive = model.IsActive;
            category.UpdatedByUser = user;

            _categoryRepository.UpdateCategory(category);
        }

        public void DeleteCategory(int categoryID)
        {
            _categoryRepository.DeleteCategory(categoryID);
        }
    }
}
