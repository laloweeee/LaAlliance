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
using Azure.Storage.Blobs;
using Azure.Storage.Sas;

namespace ASI.Basecode.Services.Services
{
    /// <summary>
    /// Service for managing products.
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly IFileHandlingService _fileHandlingService;
        private readonly IMapper _mapper;
        private readonly ICategoryService _categoryService;
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<ProductService> _logger;

        /// <summary>
        /// Constructor for ProductService.
        /// </summary>
        /// <param name="productRepository"></param>
        /// <param name="userRepository"></param>
        /// <param name="fileHandlingService"></param>
        /// <param name="mapper"></param>
        public ProductService(
                            IProductRepository productRepository,
                            IUserRepository userRepository,
                            IFileHandlingService fileHandlingService,
                            IMapper mapper,
                            ICategoryService categoryService,
                            IOrderRepository orderRepository,
                            ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _userRepository = userRepository;
            _fileHandlingService = fileHandlingService;
            _mapper = mapper;
            _categoryService = categoryService;
            _orderRepository = orderRepository;
            _logger = logger;
        }

        /// <summary>
        /// Retrieve all products.
        /// </summary>
        /// <returns></returns>
        public IQueryable<Product> GetAllProducts()
        {
            return _productRepository
                .GetAllIncludingDeleted<Product>()
                .OrderBy(p => p.IsDeleted)
                .ThenBy(p => p.ProductCategory.CategoryName)
                .ThenBy(p => p.ProductName)
                .AsQueryable();
        }

        /// <summary>
        /// Retrieve a product by its ID.
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public ProductViewModel GetProductByID(int productID)
        {
            var product = _productRepository
                .GetProducts().FirstOrDefault(p => p.ProductID == productID);

            if (product == null) throw new ArgumentException("The product does not exist.");

            
            var model = _mapper.Map<ProductViewModel>(product);
            model.CategoryName = product.ProductCategory?.CategoryName;

            _logger.LogInformation($"Retrieved product by ID: {productID}");
            _logger.LogInformation($"Product Name: {model.ProductName}, Category: {model.CategoryName}");
            _logger.LogInformation($"Product has {model.CustomizationGroups?.Count} customization groups.");

            return model;
        }

        /// <summary>
        /// Retrieve all active products.
        /// </summary>
        /// <returns></returns>
        public List<ProductViewModel> GetActiveProducts()
        {
            var products = _productRepository.GetProducts().Where(p => p.IsActive);

            // Return empty list if no products found
            if (products == null || !products.Any()) return new List<ProductViewModel>();

            var model = new List<ProductViewModel>();

            // Map each product to ProductViewModel and include CategoryName
            foreach (var product in products)
            {
                var productViewModel = _mapper.Map<ProductViewModel>(product);
                productViewModel.CategoryName = product.ProductCategory?.CategoryName;
                model.Add(productViewModel);
            }

            // Return model
            return model;
        }

        /// <summary>
        /// Retrieve all deleted products.
        /// </summary>
        /// <returns></returns>
        public List<ProductViewModel> GetDeletedProducts()
        {
            var products = _productRepository.GetAllIncludingDeleted<Product>().Where(p => p.IsDeleted);

            // Return empty list if no products found
            if (products == null || !products.Any()) return new List<ProductViewModel>();

            var model = new List<ProductViewModel>();

            // Map each product to ProductViewModel and include CategoryName
            foreach (var product in products)
            {
                var productViewModel = _mapper.Map<ProductViewModel>(product);
                productViewModel.CategoryName = product.ProductCategory?.CategoryName;
                model.Add(productViewModel);
            }

            // Return model
            return model;
        }

        /// <summary>
        /// Add a new product.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task AddProduct(ProductViewModel model)
        {
            var categoryExists = _categoryService.GetCategoryByID(model.CategoryID) ?? throw new ArgumentException("The selected category does not exist.");

            var existingProduct = _productRepository.GetProducts()
                .FirstOrDefault(p => p.ProductName.ToLower().Trim() == model.ProductName.ToLower().Trim()
                    && p.CategoryID == model.CategoryID);

            if (existingProduct != null)
            {
                throw new ArgumentException("A product with this name already exists in the selected category.");
            }

            var product = new Product
            {
                ProductName = model.ProductName.Trim(),
                ProductDescription = model.ProductDescription?.Trim(),
                ProductPrice = model.ProductPrice,
                CategoryID = model.CategoryID,
                IsActive = model.IsActive,
            };

            if (model.ImageFile != null)
            {
                var imageUrl = await _fileHandlingService.HandleFile(model.ImageFile, "products");
                product.ProductImage = imageUrl;
            }

            if (model.CustomizationGroups != null && model.CustomizationGroups.Count != 0)
            {
                product.ProductOptionGroup = [.. model.CustomizationGroups.Select(cg => new ProductOptionGroup
                {
                    OptionGroupName = cg.OptionGroupName,
                    IsRequired = cg.IsRequired,
                    NumberOfChoice = cg.NumberOfChoice,
                    ProductOptionItems = cg.ProductOptionItems?.Select(co => new ProductOptionItems
                    {
                        OptionName = co.OptionName,
                        AdditionalPrice = co.AdditionalPrice
                    }).ToList() ?? []
                })];
            }

            _productRepository.AddProduct(product);
        }

        /// <summary>
        /// Edit an existing product.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task EditProduct(ProductViewModel model)
        {
            var product = _productRepository.GetProducts().FirstOrDefault(p => p.ProductID == model.ProductID)
                ?? throw new ArgumentException("The product does not exist.");

            var categoryExists = _categoryService.GetCategoryByID(model.CategoryID)
                ?? throw new ArgumentException("The selected category does not exist.");

            var existingProduct = _productRepository.GetProducts()
                .FirstOrDefault(p => p.ProductID != model.ProductID
                    && p.ProductName.ToLower().Trim() == model.ProductName.ToLower().Trim()
                    && p.CategoryID == model.CategoryID);

            if (existingProduct != null)
            {
                throw new ArgumentException("A product with this name already exists in the selected category.");
            }

            bool hasChanges = false;

            // Update only if Name has changed
            if (product.ProductName != model.ProductName.Trim())
            {
                product.ProductName = model.ProductName.Trim();
                hasChanges = true;
            }

            // Update only if Description has changed
            var newDescription = model.ProductDescription?.Trim();
            if (product.ProductDescription != newDescription)
            {
                product.ProductDescription = newDescription;
                hasChanges = true;
            }

            // Update only if Price has changed
            if (product.ProductPrice != model.ProductPrice)
            {
                product.ProductPrice = model.ProductPrice;
                hasChanges = true;
            }

            // Update only if CategoryID has changed
            if (product.CategoryID != model.CategoryID)
            {
                product.CategoryID = model.CategoryID;
                hasChanges = true;
            }

            // Update only if IsActive has changed
            if (product.IsActive != model.IsActive)
            {
                product.IsActive = model.IsActive;
                hasChanges = true;
            }

            // Update image only if a new file is provided
            if (model.ImageFile != null)
            {
                var imageUrl = await _fileHandlingService.HandleFile(model.ImageFile, "products");

                if (product.ProductImage != imageUrl)
                {
                    product.ProductImage = imageUrl;
                    hasChanges = true;
                }
            }

            // Update Customization Groups only if they have changed
            if (model.CustomizationGroups != null)
            {
                bool customizationsChanged = HasCustomizationGroupsChanged(product.ProductOptionGroup.ToList(), model.CustomizationGroups);

                if (customizationsChanged)
                {
                    // Remove existing groups and options
                    product.ProductOptionGroup.Clear();

                    // Add updated groups and options
                    foreach (var cg in model.CustomizationGroups)
                    {
                        var newGroup = new ProductOptionGroup
                        {
                            OptionGroupName = cg.OptionGroupName,
                            IsRequired = cg.IsRequired,
                            NumberOfChoice = cg.NumberOfChoice,
                            ProductOptionItems = cg.ProductOptionItems?.Select(co => new ProductOptionItems
                            {
                                OptionName = co.OptionName,
                                AdditionalPrice = co.AdditionalPrice
                            }).ToList() ?? new List<ProductOptionItems>()
                        };
                        product.ProductOptionGroup.Add(newGroup);
                    }
                    hasChanges = true;
                }
            }
            else if (product.ProductOptionGroup.Any())
            {
                // If no customization groups provided but product had some, clear them
                product.ProductOptionGroup.Clear();
                hasChanges = true;
            }

            // Only update if there are changes
            if (hasChanges)
            {
                _productRepository.UpdateProduct(product);
            }
        }

        /// <summary>
        /// Delete a product.
        /// </summary>
        /// <param name="productID"></param>
        /// <param name="deletedBy"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public void DeleteProduct(int productID, string deletedBy)
        {
            var product = _productRepository.GetProducts().FirstOrDefault(p => p.ProductID == productID);

            // Validate product existence
            if (product == null) throw new ArgumentException("The product does not exist.");

            // Validate product is not active
            if (product.IsActive) throw new InvalidOperationException("Cannot delete product that is currently active.");

            // Validate product is not associated with any orders
            if (_orderRepository.IsProductInAnyOrder(productID)) throw new InvalidOperationException("Cannot delete product that is associated with existing orders.");

            _productRepository.SoftDelete(product, deletedBy);
        }

        /// <summary>
        /// Recover a deleted product.
        /// </summary>
        /// <param name="productID"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public void RecoverProduct(int productID)
        {
            var product = _productRepository.GetAllIncludingDeleted<Product>().FirstOrDefault(p => p.ProductID == productID);

            // Validate product existence
            if (product == null) throw new ArgumentException("The product does not exist.");

            // Validate product is deleted
            if (!product.IsDeleted) throw new InvalidOperationException("The product is not deleted.");

            _productRepository.Restore(product);
        }

        /// <summary>
        /// Permanently delete a product.
        /// </summary>
        /// <param name="productID"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public void PermanentDelete(int productID)
        {
            var product = _productRepository.GetAllIncludingDeleted<Product>().FirstOrDefault(p => p.ProductID == productID);

            // Validate product existence
            if (product == null) throw new ArgumentException("The product does not exist.");

            // Validate product is deleted
            if (!product.IsDeleted) throw new InvalidOperationException("The product is not deleted.");

            // Validate product is not associated with any orders
            if (_orderRepository.IsProductInAnyOrder(productID)) throw new InvalidOperationException("Cannot permanently delete product that is associated with existing orders.");

            _productRepository.HardDelete(product);
        }

        /// <summary>
        /// Check if customization groups have changed.
        /// </summary>
        /// <param name="existingGroups"></param>
        /// <param name="newGroups"></param>
        /// <returns></returns>
        private static bool HasCustomizationGroupsChanged(List<ProductOptionGroup> existingGroups, List<ProductOptionGroupViewModel> newGroups)
        {
            if (existingGroups.Count != newGroups.Count)
                return true;

            for (int i = 0; i < existingGroups.Count; i++)
            {
                var existing = existingGroups[i];
                var newGroup = newGroups.FirstOrDefault(g => g.OptionGroupName == existing.OptionGroupName);

                if (newGroup == null)
                    return true;

                // Check group properties
                if (existing.OptionGroupName != newGroup.OptionGroupName ||
                    existing.IsRequired != newGroup.IsRequired ||
                    existing.NumberOfChoice != newGroup.NumberOfChoice)
                    return true;

                // Check options count
                var existingOptions = existing.ProductOptionItems?.ToList() ?? new List<ProductOptionItems>();
                var newOptions = newGroup.ProductOptionItems ?? new List<ProductOptionItemViewModel>();

                if (existingOptions.Count != newOptions.Count)
                    return true;

                // Compare each option
                foreach (var existingOption in existingOptions)
                {
                    var newOption = newOptions.FirstOrDefault(o => o.OptionName == existingOption.OptionName);

                    if (newOption == null ||
                        existingOption.OptionName != newOption.OptionName ||
                        existingOption.AdditionalPrice != newOption.AdditionalPrice)
                        return true;
                }
            }

            return false;
        }

    }
}