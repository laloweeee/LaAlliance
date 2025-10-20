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
using ASI.Basecode.WebApp.Helpers;

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

        /// <summary>
        /// Renders the main menu management page with categories and products.
        /// </summary>
        /// <param name="activeTab"></param>
        /// <returns></returns>
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
                this.ShowErrorToast("Error loading menu: " + ex.Message);
                return View(new MenuViewModel());
            }
        }

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
                this.ShowErrorToast("Error loading categories: " + ex.Message);
                ViewBag.Categories = new List<ProductCategory>();
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
                    this.ShowSuccessToast("Category added successfully!");
                    return RedirectToAction("Index", new { activeTab = returnTab });
                }
                catch (Exception ex)
                {
                    this.ShowErrorToast("Error adding category: " + ex.Message);
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
                    this.ShowSuccessToast("Category updated successfully!");
                }
                catch (Exception ex)
                {
                    this.ShowErrorToast("Error updating category: " + ex.Message);
                }
            }

            ViewBag.ReturnTab = returnTab;
            return RedirectToAction("CategoryForm", new { categoryID = model.CategoryID, returnTab });
        }

        /// <summary>
        /// Handles the deletion of a category.
        /// </summary>
        /// <param name="categoryID"></param>
        /// <param name="returnTab"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCategory(int id, string returnTab = "categories")
        {
            ViewBag.ReturnTab = returnTab;
            
            try
            {
                var category = _categoryService.GetCategoryByID(id);

                _categoryService.DeleteCategory(id, User.FindFirstValue(ClaimTypes.Name));

                this.ShowSuccessToast("Category deleted successfully!");

                return RedirectToAction("Index", new { activeTab = returnTab });
            }
            catch (Exception ex)
            {
                this.ShowErrorToast("Error deleting category: " + ex.Message);
                
                return RedirectToAction("CategoryForm", new { categoryID = id, returnTab });
            }
        }

        /// <summary>
        /// Handles the recovery of a deleted category.
        /// </summary>
        /// <param name="categoryID"></param>
        /// <returns></returns>
        public IActionResult RecoverCategory(int id)
        {
            _logger.LogInformation($"RecoverCategory action called with ID: {id}");
            try
            {
                _categoryService.RecoverCategory(id);
                this.ShowSuccessToast("Category recovered successfully!");
            }
            catch (Exception ex)
            {
                this.ShowErrorToast("Error recovering category: " + ex.Message);
            }

            return RedirectToAction("Index", new { activeTab = "categories" });
        }

        /// <summary>
        /// Handles the permanent deletion of a category.
        /// </summary>
        /// <param name="categoryID"></param>
        /// <returns></returns>
        public IActionResult HardDeleteCategory(int id)
        {
            try
            {
                var category = _categoryService.GetCategoryByID(id);

                _categoryService.PermanentDeleteCategory(id);

                this.ShowSuccessToast("Category permanently deleted successfully!");
            }
            catch (Exception ex)
            {
                this.ShowErrorToast("Error permanently deleting category: " + ex.Message);
            }

            return RedirectToAction("Index", new { activeTab = "categories" });
        }

        #endregion

        #region Product Actions

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

                return View(product);
            }
            catch (Exception ex)
            {
                this.ShowErrorToast("Error loading product: " + ex.Message);
            }
            
            return RedirectToAction("ProductForm", new { productID = productID.Value, returnTab });
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
            PopulateViewBagCategories();
            try
            {
                if (ModelState.IsValid)
                {
                    await _productService.AddProduct(model);
                }

                this.ShowSuccessToast("Product added successfully!");
                return RedirectToAction("Index", new { activeTab = returnTab });
            }
            catch (Exception ex)
            {
                this.ShowErrorToast("Error adding product: " + ex.Message);
                ViewBag.ReturnTab = returnTab;

                return RedirectToAction("ProductForm", new { productID = model.ProductID, returnTab });
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
            PopulateViewBagCategories();
            try
            {
                if (ModelState.IsValid)
                {
                    await _productService.EditProduct(model);

                    this.ShowSuccessToast("Product updated successfully!");
                }

                ViewBag.ReturnTab = returnTab;
            }
            catch (Exception ex)
            {
                this.ShowErrorToast("Error updating product: " + ex.Message);
                ViewBag.ReturnTab = returnTab;
            }

            return RedirectToAction("ProductForm", new { productID = model.ProductID, returnTab });
        }

        /// <summary>
        /// Handles the deletion of a category.
        /// </summary>
        /// <param name="categoryID"></param>
        /// <param name="returnTab"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteProduct(int id, string returnTab = "products")
        {
            try
            {
                var product = _productService.GetProductByID(id);

                _productService.DeleteProduct(id, User.FindFirstValue(ClaimTypes.Name));

                this.ShowSuccessToast("Product deleted successfully!");
            }
            catch (Exception ex)
            {
                this.ShowErrorToast("Error deleting product: " + ex.Message);
                ViewBag.ReturnTab = returnTab;
            }

            return RedirectToAction("ProductForm", new { productID = id, returnTab });
        }

        /// <summary>
        /// Handles the recovery of a deleted category.
        /// </summary>
        /// <param name="categoryID"></param>
        /// <returns></returns>
        public IActionResult RecoverProduct(int id)
        {
            try
            {
                _productService.RecoverProduct(id);
                this.ShowSuccessToast("Product recovered successfully!");
            }
            catch (Exception ex)
            {   
                // Log the exception for debugging purposes
                this.ShowErrorToast("Error recovering product: " + ex.Message);
            }

            // Redirect back to the products tab
            return RedirectToAction("Index", new { activeTab = "products" });
        }

        /// <summary>
        /// Handles the permanent deletion of a category.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult HardDeleteProduct(int id)
        {
            try
            {
                var product = _productService.GetProductByID(id);

                _productService.PermanentDelete(id);

                this.ShowSuccessToast("Product permanently deleted successfully!");
            }
            catch (Exception ex)
            {
                this.ShowErrorToast("Error permanently deleting product: " + ex.Message);
            }

            return RedirectToAction("Index", new { returnTab = "products" });
        }

        /// <summary>
        /// Renders the recently deleted items page with both deleted categories and products.
        /// </summary>
        /// <returns></returns>
        public IActionResult RecentlyDeleted()
        {
            try
            {
                var deletedCategories = _categoryService.GetAllCategories()
                    .Where(c => c.IsDeleted)
                    .ToList();
                
                var deletedProducts = _productService.GetAllProducts()
                    .Where(p => p.IsDeleted)
                    .ToList();

                var model = new MenuViewModel
                {
                    Categories = deletedCategories,
                    Products = deletedProducts
                };

                return View(model);
            }
            catch (Exception ex)
            {
                this.ShowErrorToast("Error loading deleted items: " + ex.Message);
                return View(new MenuViewModel());
            }
        }

        #endregion
    }    
}
