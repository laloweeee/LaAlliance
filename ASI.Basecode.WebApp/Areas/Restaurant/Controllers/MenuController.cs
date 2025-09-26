using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.WebApp.Areas.Restaurant.Controllers
{
    [Authorize(Policy = "Restaurant")]
    [Area("Restaurant")]
    public class MenuController : ControllerBase<MenuController>
    {
        private readonly ICategoryService _categoryService;
        private readonly IUserService _userService;

        public MenuController(
                                        IHttpContextAccessor httpContextAccessor,
                                        ILoggerFactory loggerFactory,
                                        IConfiguration configuration,
                                        IMapper mapper,
                                        ICategoryService categoryService,
                                        IUserService userService
                                    ) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _categoryService = categoryService;
            _userService = userService;
        }

        public IActionResult Index()
        {
            try
            {
                var categories = _categoryService.GetAllCategories().ToList();
                return View(categories);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading categories: " + ex.Message;
                return View(new List<Category>());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddCategory(CategoryViewModel category)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please enter a valid category name.";
                return RedirectToAction("Index");
            }

            try
            {
                _categoryService.AddCategory(category, UserId);
                TempData["SuccessMessage"] = "Category added successfully!";
            }
            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error adding category: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditCategory(CategoryViewModel category)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please enter a valid category name.";
                return RedirectToAction("Index");
            }

            try
            {
                _categoryService.UpdateCategory(category, UserId);
                TempData["SuccessMessage"] = "Category updated successfully!";
            }
            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error updating category: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCategory(int categoryID)
        {
            try
            {
                // TODO: Add validation to check if the category is associated with any menu items before deletion
                _categoryService.DeleteCategory(categoryID);
                TempData["SuccessMessage"] = "Category deleted successfully!";
            }
            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error deleting category: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleVisibility(int categoryID)
        {
            try
            {
                // TODO: Implement category visibility toggle logic
                TempData["SuccessMessage"] = "Category visibility updated successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error updating category visibility: " + ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}
