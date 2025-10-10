using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.WebApp.Areas.Restaurant.Models;
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
using System.Security.Claims;
using System.Threading.Tasks;

namespace ASI.Basecode.WebApp.Areas.Restaurant.Controllers
{
    [Authorize(Policy = "Restaurant")]
    [Area("Restaurant")]
    public class MenuController : ControllerBase<MenuController>
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public MenuController(
            IHttpContextAccessor httpContextAccessor,
            ILoggerFactory loggerFactory,
            IConfiguration configuration,
            IMapper mapper,
            ICategoryService categoryService,
            IProductService productService
        ) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _categoryService = categoryService;
            _productService = productService;
        }

        public IActionResult Index(string activeTab = "products")
        {
            try
            {
                var categories = _categoryService.GetAllCategories().ToList();
                var products = _productService.GetAllProducts().ToList();

                ViewBag.Categories = categories;
                ViewBag.Products = products;
                ViewBag.ActiveTab = activeTab;

                var model = new MenuViewModel
                {
                    Categories = categories,
                    Products = products
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ToastrError"] = "Error loading menu: " + ex.Message;
                return View(new MenuViewModel());
            }
        }

        #region Category Actions

        [HttpGet]
        public IActionResult CategoryForm(int? categoryID, string returnTab = "categories")
        {
            ViewBag.ReturnTab = returnTab;

            if (categoryID.HasValue && categoryID.Value == 0)
            {
                return View(new CategoryViewModel());
            }

            if (!categoryID.HasValue)
            {
                return View(new CategoryViewModel());
            }

            var category = _categoryService.GetCategoryByID(categoryID.Value);
            if (category == null)
            {
                return NotFound();
            }

            var model = _mapper.Map<CategoryViewModel>(category);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddCategory(CategoryViewModel model, string returnTab = "categories")
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userID = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                    _categoryService.AddCategory(model, userID);
                    TempData["ToastrSuccess"] = "Category added successfully!";
                    return RedirectToAction("Index", new { activeTab = returnTab });
                }
                catch (Exception ex)
                {
                    TempData["ToastrError"] = ex.Message;
                }
            }

            ViewBag.ReturnTab = returnTab;
            return View("CategoryForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateCategory(CategoryViewModel model, string returnTab = "categories")
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userID = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                    _categoryService.UpdateCategory(model, userID);
                    TempData["ToastrSuccess"] = "Category updated successfully!";
                    return RedirectToAction("Index", new { activeTab = returnTab });
                }
                catch (Exception ex)
                {
                    TempData["ToastrError"] = ex.Message;
                }
            }

            ViewBag.ReturnTab = returnTab;
            return View("CategoryForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCategory(int categoryID, string returnTab = "categories")
        {
            try
            {
                _categoryService.DeleteCategory(categoryID);
                TempData["ToastrSuccess"] = "Category deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ToastrError"] = ex.Message;
            }

            return RedirectToAction("Index", new { activeTab = returnTab });
        }

        #endregion

        #region Product Actions

        [HttpGet]
        public IActionResult ProductForm(int? productID, string returnTab = "products")
        {
            ViewBag.ReturnTab = returnTab;
            ViewBag.Categories = _categoryService.GetAllCategories().ToList();

            // Handle new product creation
            if (!productID.HasValue || productID.Value == 0)
            {
                return View(new ProductViewModel());
            }

            try
            {
                // Get the product from the service
                var product = _productService.GetProductByID(productID.Value).FirstOrDefault();
                
                if (product == null)
                {
                    TempData["ToastrError"] = "Product not found.";
                    return RedirectToAction("Index", new { activeTab = returnTab });
                }

                // Map the product to view model
                var model = _mapper.Map<ProductViewModel>(product);
                
                _logger.LogInformation($"Loading Product ID: {model.ProductID}, Name: {model.Name}, Category: {model.CategoryID}");
                _logger.LogInformation($"Customization Groups: {model.CustomizationGroups?.Count ?? 0}");
                
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading product with ID: {productID}");
                TempData["ToastrError"] = "Error loading product: " + ex.Message;
                return RedirectToAction("Index", new { activeTab = returnTab });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(ProductViewModel model, string returnTab = "products")
        {
            try
            {
                _logger.LogInformation($"Received Product: {model.Name}, CategoryID: {model.CategoryID}");
                _logger.LogInformation($"Customization Groups Count: {model.CustomizationGroups?.Count ?? 0}");

                if (model.CustomizationGroups != null)
                {
                    foreach (var group in model.CustomizationGroups)
                    {
                        _logger.LogInformation($"Group: {group.CustomizationName}, Options: {group.CustomizationOptions?.Count ?? 0}");
                    }
                }

                if (ModelState.IsValid)
                {
                    await _productService.AddProduct(model, UserId);

                    TempData["ToastrSuccess"] = "Product added successfully!";
                    return RedirectToAction("Index", new { activeTab = returnTab });
                }

                ViewBag.Categories = _categoryService.GetAllCategories().ToList();
                ViewBag.ReturnTab = returnTab;
                return View("ProductForm", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding product");
                TempData["ToastrError"] = "Error adding product: " + ex.Message;
                ViewBag.Categories = _categoryService.GetAllCategories().ToList();
                ViewBag.ReturnTab = returnTab;
                return View("ProductForm", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProduct(ProductViewModel model, string returnTab = "products")
        {
            try
            {
                _logger.LogInformation($"Updating Product ID: {model.ProductID}, Name: {model.Name}, CategoryID: {model.CategoryID}");
                _logger.LogInformation($"Customization Groups Count: {model.CustomizationGroups?.Count ?? 0}");

                if (model.CustomizationGroups != null)
                {
                    foreach (var group in model.CustomizationGroups)
                    {
                        _logger.LogInformation($"Group: {group.CustomizationName}, Options: {group.CustomizationOptions?.Count ?? 0}");
                    }
                }

                if (ModelState.IsValid)
                {
                    await _productService.EditProduct(model, UserId);

                    TempData["ToastrSuccess"] = "Product updated successfully!";
                    return RedirectToAction("Index", new { activeTab = returnTab });
                }

                ViewBag.Categories = _categoryService.GetAllCategories().ToList();
                ViewBag.ReturnTab = returnTab;
                return View("ProductForm", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product");
                TempData["ToastrError"] = "Error updating product: " + ex.Message;
                ViewBag.Categories = _categoryService.GetAllCategories().ToList();
                ViewBag.ReturnTab = returnTab;
                return View("ProductForm", model);
            }
        }

        #endregion
    }    
}
