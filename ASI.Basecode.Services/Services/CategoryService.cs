using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Manager;
using ASI.Basecode.Services.ServiceModels;
using AutoMapper;
using Microsoft.AspNetCore.Http;
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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IUserRepository userRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _categoryRepository = categoryRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public IQueryable<Category> GetAllCategories()
        {
            return _categoryRepository.GetAllCategories();
        }

        public IQueryable<Category> CategoryGetByID(int categoryID)
        {
            return _categoryRepository.GetCategoryByID(categoryID).Where(e => e.CategoryID == categoryID);
        }

        public void AddCategory(CategoryViewModel model, int userID)
        {
            // Check if category name already exists
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
                CreatedByUser = user,
                UpdatedByUser = user
            };

            _categoryRepository.AddCategory(category);
        }

        public void UpdateCategory(CategoryViewModel model, int userID)
        {
            var category = _categoryRepository.GetCategoryByID(model.CategoryID).FirstOrDefault() ?? throw new ArgumentException("Category not found.");

            var existingCategory = _categoryRepository.GetAllCategories()
                .FirstOrDefault(c => c.Name.ToLower().Trim() == model.Name.ToLower().Trim() && c.CategoryID != model.CategoryID);

            if (existingCategory != null)
            {
                throw new ArgumentException("A category with this name already exists.");
            }

            var user = _userRepository.GetUserById(userID).FirstOrDefault() ?? throw new ArgumentException("Invalid UpdatedBy user ID.");
            category.Name = model.Name.Trim();
            category.UpdatedByUser = user;
            _categoryRepository.UpdateCategory(category);
        }

        public void DeleteCategory(int categoryID)
        {
            _categoryRepository.DeleteCategory(categoryID);
        }
    }
}
