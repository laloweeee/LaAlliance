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

        /// <summary>
        /// Renders the form for adding or editing a category.
        /// </summary>
        /// <param name="categoryID"></param>
        /// <param name="returnTab"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Handles the submission of the category form for adding a new category.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnTab"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddCategory(CategoryViewModel model, string returnTab = "categories")
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _categoryService.AddCategory(model);
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

        /// <summary>
        /// Handles the submission of the category form for updating an existing category.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnTab"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateCategory(CategoryViewModel model, string returnTab = "categories")
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _categoryService.UpdateCategory(model);
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

        /// <summary>
        /// Handles the deletion of a category.
        /// </summary>
        /// <param name="categoryID"></param>
        /// <param name="returnTab"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCategory(CategoryViewModel model, string returnTab = "categories")
        {
            try
            {
                _categoryService.DeleteCategory(model.CategoryID, User.FindFirstValue(ClaimTypes.Name));
                TempData["ToastrSuccess"] = "Category deleted successfully!";
                return RedirectToAction("Index", new { activeTab = returnTab });

            }
            catch (Exception ex)
            {
                TempData["ToastrError"] = ex.Message;
                ViewBag.ReturnTab = returnTab;

                var category = _categoryService.GetCategoryByID(model.CategoryID);

                if (category == null)
                {
                    TempData["ToastrError"] = "Category not found.";
                    return RedirectToAction("Index", new { activeTab = returnTab });
                }

                var categoryViewModel = _mapper.Map<CategoryViewModel>(category);
                return View("CategoryForm", categoryViewModel);
            }
        }

        #endregion

        #region Product Actions

        /// <summary>
        /// Populates the ViewBag with categories for dropdown lists.
        /// </summary>
        private void PopulateViewBagCategories()
        {
            try
            {
                var categories = _categoryService.GetAllCategories().ToList();
                ViewBag.Categories = categories;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error populating categories in ViewBag");
                ViewBag.Categories = new List<ProductCategory>();
            }
        }

        /// <summary>
        /// Handles the submission of the product form for adding a new product.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnTab"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult ProductForm(int? productID, string returnTab = "products")
        {
            ViewBag.ReturnTab = returnTab;
            PopulateViewBagCategories();

            if (!productID.HasValue || productID.Value == 0)
            {
                return View(new ProductViewModel());
            }

            try
            {
                var product = _productService.GetProductByID(productID.Value);
                
                if (product == null)
                {
                    TempData["ToastrError"] = "Product not found.";
                    return RedirectToAction("Index", new { activeTab = returnTab });
                }
             
                return View(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading product with ID: {productID}");
                TempData["ToastrError"] = "Error loading product: " + ex.Message;
                return RedirectToAction("Index", new { activeTab = returnTab });
            }
        }

        /// <summary>
        /// Handles the submission of the product form for adding a new product.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnTab"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(ProductViewModel model, string returnTab = "products")
        {
            try
            {
                _logger.LogInformation($"Received Product: {model.ProductName}, CategoryID: {model.CategoryID}");
                _logger.LogInformation($"Customization Groups Count: {model.CustomizationGroups?.Count ?? 0}");

                if (model.CustomizationGroups != null)
                {
                    foreach (var group in model.CustomizationGroups)
                    {
                        _logger.LogInformation($"Group: {group.OptionGroupName}, Options: {group.ProductOptionItems?.Count ?? 0}");
                    }
                }

                if (ModelState.IsValid)
                {
                    await _productService.AddProduct(model);

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

        /// <summary>
        /// Handles the submission of the product form for adding a new product.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnTab"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProduct(ProductViewModel model, string returnTab = "products")
        {
            try
            {
                _logger.LogInformation($"Updating Product ID: {model.ProductID}, Name: {model.ProductName}, CategoryID: {model.CategoryID}");
                _logger.LogInformation($"Customization Groups Count: {model.CustomizationGroups?.Count ?? 0}");

                if (model.CustomizationGroups != null)
                {
                    foreach (var group in model.CustomizationGroups)
                    {
                        _logger.LogInformation($"Group: {group.OptionGroupName}, Options: {group.ProductOptionItems?.Count ?? 0}");
                    }
                }

                if (ModelState.IsValid)
                {
                    await _productService.EditProduct(model);

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
