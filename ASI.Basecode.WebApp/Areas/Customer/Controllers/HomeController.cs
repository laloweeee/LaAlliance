using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace ASI.Basecode.WebApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : ControllerBase<HomeController>
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private new readonly ICartService _cartService;

        public HomeController(
            IHttpContextAccessor httpContextAccessor,
            ILoggerFactory loggerFactory,
            IConfiguration configuration,
            IProductService productService,
            ICategoryService categoryService,
            ICartService cartService,
            IMapper mapper = null)
            : base(httpContextAccessor, loggerFactory, configuration, mapper, cartService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _cartService = cartService;
        }

        /// <summary>
        /// Home page showing products and categories
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            var model = new Models.HomeViewModel
            {
                Categories = _categoryService.GetAllCategories().ToList().AsQueryable(),
                Products = _productService.GetActiveProducts()
            };

            ViewBag.UserName = User.FindFirst(ClaimTypes.Name)?.Value;
            return View(model);
        }

        /// <summary>
        /// View to add product to cart
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult ProductToCart(int productID)
        {
            var product = _productService.GetProductByID(productID);
            
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        /// <summary>
        /// View to edit existing cart item
        /// </summary>
        /// <param name="cartItemID"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult EditCartItem(int cartItemID)
        {
            try
            {
                var cartItem = _cartService.GetCartItemForEdit(cartItemID, UserId);
                
                _logger.LogInformation($"CartItem found: {cartItem != null}, ProductID: {cartItem?.ProductID}");
                
                if (cartItem?.CartItemOptions != null)
                {
                    _logger.LogInformation($"CartItem has {cartItem.CartItemOptions.Count} options:");
                    foreach (var option in cartItem.CartItemOptions)
                    {
                        _logger.LogInformation($"- GroupID: {option.ProductOptionGroupID}, ItemID: {option.ProductOptionItemID}, Name: {option.OptionName}");
                    }
                }
                var product = _productService.GetProductByID(cartItem.ProductID);

                if (product == null)
                {
                    _logger.LogWarning($"Product not found for ID: {cartItem.ProductID}");
                    return NotFound();
                }

                // Pre-select existing options and quantity
                ViewBag.UserName = User.FindFirst(ClaimTypes.Name)?.Value;
                ViewBag.IsEdit = true;
                ViewBag.CartItemID = cartItemID;
                ViewBag.ExistingQuantity = cartItem.Quantity;
                
                var selectedOptions = cartItem.CartItemOptions
                    .GroupBy(o => o.ProductOptionGroupID)
                    .ToDictionary(g => g.Key, g => g.Select(o => o.ProductOptionItemID).ToList());
                
                ViewBag.SelectedOptions = selectedOptions;
                
                _logger.LogInformation($"Selected options prepared: {selectedOptions.Count} groups");
                foreach (var optionGroup in selectedOptions)
                {
                    _logger.LogInformation($"- Group {optionGroup.Key}: {string.Join(", ", optionGroup.Value)}");
                }

                return View("ProductToCart", product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading cart item for edit");
                TempData["ErrorMessage"] = "Unable to load item for editing.";
                return RedirectToAction("Index", "Cart");
            }
        }

        

        /// <summary>
        /// Add or update item in cart
        /// </summary>
        /// <param name="productID"></param>
        /// <param name="quantity"></param>
        /// <param name="cartItemID"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddToCart(int productID, int quantity, int? cartItemID)
        {
            try
            {
                // Extract selected options from form
                var selectedOptions = ExtractSelectedOptions(Request.Form);

                var request = new AddToCartRequest
                {
                    ProductID = productID,
                    Quantity = quantity,
                    SelectedOptions = selectedOptions
                };

                if (cartItemID.HasValue && cartItemID.Value > 0)
                {
                    // Edit mode: Remove old item and add new one
                    _cartService.RemoveCartItem(cartItemID.Value, UserId);
                }

                _cartService.AddItemToCart(request, UserId);

                TempData["SuccessMessage"] = cartItemID.HasValue
                    ? "Cart item updated successfully!"
                    : "Item added to cart successfully!";

                return RedirectToAction("ProductToCart", new { productID });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding/updating item in cart");
                TempData["ErrorMessage"] = "An error occurred. Please try again.";
                return RedirectToAction("ProductToCart", new { productID });
            }
        }

        /// <summary>
        /// Extract selected options from form data
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        private Dictionary<int, List<int>> ExtractSelectedOptions(IFormCollection form)
        {
            var selectedOptions = new Dictionary<int, List<int>>();

            foreach (var key in form.Keys.Where(k => k.StartsWith("option_")))
            {
                if (int.TryParse(key.Replace("option_", ""), out int groupId))
                {
                    // Get all values for this key (checkboxes/radios may have multiple values)
                    var formValues = form[key];
                    var values = new List<int>();

                    foreach (var value in formValues)
                    {
                        // Each value might be a single ID or comma-separated IDs
                        var ids = value.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var id in ids)
                        {
                            if (int.TryParse(id.Trim(), out int optionItemId))
                            {
                                values.Add(optionItemId);
                            }
                        }
                    }

                    if (values.Any())
                    {
                        selectedOptions[groupId] = values.Distinct().ToList(); // Use Distinct to avoid duplicates
                    }
                }
            }

            // Log for debugging
            _logger.LogInformation($"Extracted options: {string.Join(", ", selectedOptions.Select(kvp => $"Group {kvp.Key}: [{string.Join(", ", kvp.Value)}]"))}");

            return selectedOptions;
        }

        /// <summary>
        /// Map product data model to view model
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        private static ProductViewModel MapProductToViewModel(Data.Models.Product product)
        {
            return new ProductViewModel
            {
                ProductID = product.ProductID,
                ProductName = product.ProductName,
                ProductDescription = product.ProductDescription,
                ProductPrice = product.ProductPrice,
                ProductImage = product.ProductImage,
                CustomizationGroups = product.ProductOptionGroup.Select(g => new ProductOptionGroupViewModel
                {
                    ProductOptionGroupID = g.ProductOptionGroupID,
                    ProductID = g.ProductID ?? 0,
                    OptionGroupName = g.OptionGroupName,
                    IsRequired = g.IsRequired,
                    NumberOfChoice = g.NumberOfChoice,
                    ProductOptionItems = g.ProductOptionItems.Select(i => new ProductOptionItemViewModel
                    {
                        ProductOptionItemsID = i.ProductOptionItemsID,
                        ProductOptionGroupID = i.ProductOptionGroupID,
                        OptionName = i.OptionName,
                        AdditionalPrice = i.AdditionalPrice
                    }).ToList()
                }).ToList()
            };
        }
    }
}