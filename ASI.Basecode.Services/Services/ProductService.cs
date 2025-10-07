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
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly IFileHandlingService _fileHandlingService;
        private readonly IMapper _mapper;
        private readonly ICategoryService _categoryService;
        private readonly BlobContainerClient _blobContainerClient;

        public ProductService(IProductRepository productRepository, IUserRepository userRepository, IFileHandlingService fileHandlingService, IMapper mapper, ICategoryService categoryService, BlobContainerClient blobContainerClient)
        {
            _productRepository = productRepository;
            _userRepository = userRepository;
            _fileHandlingService = fileHandlingService;
            _mapper = mapper;
            _categoryService = categoryService;
            _blobContainerClient = blobContainerClient;
        }

        public IQueryable<Product> GetAllProducts()
        {
            return _productRepository.GetProducts();
        }

        public IQueryable<Product> GetProductByID(int productID)
        {
            return _productRepository.GetProductByID(productID);
        }

        public IQueryable<Product> GetProductsByCategoryID(int categoryID)
        {
            return _productRepository.GetProductsByCategoryID(categoryID);
        }

        public IQueryable<Product> GetActiveProducts()
        {
            return _productRepository.GetActiveProducts();
        }

        public async Task AddProduct(ProductViewModel model, int userID)
        {
            var categoryExists = _categoryService.GetCategoryByID(model.CategoryID) ?? throw new ArgumentException("The selected category does not exist.");

            var existingProduct = _productRepository.GetProducts()
                .FirstOrDefault(p => p.Name.ToLower().Trim() == model.Name.ToLower().Trim()
                    && p.CategoryID == model.CategoryID);

            if (existingProduct != null)
            {
                throw new ArgumentException("A product with this name already exists in the selected category.");
            }

            var user = _userRepository.GetUserById(userID).FirstOrDefault() ?? throw new ArgumentException("Invalid user ID.");

            var product = new Product
            {
                Name = model.Name.Trim(),
                Description = model.Description?.Trim(),
                Price = model.Price,
                CategoryID = model.CategoryID,
                IsActive = model.IsActive,
                CreatedByUser = user,
                UpdatedByUser = user,
            };

            if (model.ImageFile != null)
            {
                var imageUrl = await _fileHandlingService.HandleFile(model.ImageFile, "products");
                product.ImageUrl = imageUrl;
            }

            if (model.CustomizationGroups != null && model.CustomizationGroups.Count != 0)
            {
                product.CustomizationGroups = [.. model.CustomizationGroups.Select(cg => new CustomizationGroup
                {
                    CustomizationName = cg.CustomizationName,
                    IsRequired = cg.IsRequired,
                    NumberOfChoice = cg.NumberOfChoice,
                    CustomizationOptions = cg.CustomizationOptions?.Select(co => new CustomizationOption
                    {
                        OptionName = co.OptionName,
                        AdditionalPrice = co.AdditionalPrice
                    }).ToList() ?? []
                })];
            }

            _productRepository.AddProduct(product);
        }

        public async Task EditProduct(ProductViewModel model, int userID)
        {
            var product = _productRepository.GetProductByID(model.ProductID).FirstOrDefault()
                ?? throw new ArgumentException("The product does not exist.");

            var categoryExists = _categoryService.GetCategoryByID(model.CategoryID)
                ?? throw new ArgumentException("The selected category does not exist.");

            var existingProduct = _productRepository.GetProducts()
                .FirstOrDefault(p => p.ProductID != model.ProductID
                    && p.Name.ToLower().Trim() == model.Name.ToLower().Trim()
                    && p.CategoryID == model.CategoryID);

            if (existingProduct != null)
            {
                throw new ArgumentException("A product with this name already exists in the selected category.");
            }

            bool hasChanges = false;

            // Update only if Name has changed
            if (product.Name != model.Name.Trim())
            {
                product.Name = model.Name.Trim();
                hasChanges = true;
            }

            // Update only if Description has changed
            var newDescription = model.Description?.Trim();
            if (product.Description != newDescription)
            {
                product.Description = newDescription;
                hasChanges = true;
            }

            // Update only if Price has changed
            if (product.Price != model.Price)
            {
                product.Price = model.Price;
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

                if (product.ImageUrl != imageUrl)
                {
                    product.ImageUrl = imageUrl;
                    hasChanges = true;
                }
            }

            // Update Customization Groups only if they have changed
            if (model.CustomizationGroups != null)
            {
                bool customizationsChanged = HasCustomizationGroupsChanged(product.CustomizationGroups.ToList(), model.CustomizationGroups);

                if (customizationsChanged)
                {
                    // Remove existing groups and options
                    product.CustomizationGroups.Clear();

                    // Add updated groups and options
                    foreach (var cg in model.CustomizationGroups)
                    {
                        var newGroup = new CustomizationGroup
                        {
                            CustomizationName = cg.CustomizationName,
                            IsRequired = cg.IsRequired,
                            NumberOfChoice = cg.NumberOfChoice,
                            CustomizationOptions = cg.CustomizationOptions?.Select(co => new CustomizationOption
                            {
                                OptionName = co.OptionName,
                                AdditionalPrice = co.AdditionalPrice
                            }).ToList() ?? []
                        };
                        product.CustomizationGroups.Add(newGroup);
                    }
                    hasChanges = true;
                }
            }
            else if (product.CustomizationGroups.Any())
            {
                // If no customization groups provided but product had some, clear them
                product.CustomizationGroups.Clear();
                hasChanges = true;
            }

            // Only update if there are changes
            if (hasChanges)
            {
                var user = _userRepository.GetUserById(userID).FirstOrDefault()
                    ?? throw new ArgumentException("Invalid user ID.");

                product.UpdatedByUser = user;
                product.UpdatedTime = DateTime.Now; // If you have this field

                _productRepository.UpdateProduct(product);
            }
        }

        private static bool HasCustomizationGroupsChanged(List<CustomizationGroup> existingGroups, List<CustomizationGroupViewModel> newGroups)
        {
            if (existingGroups.Count != newGroups.Count)
                return true;

            for (int i = 0; i < existingGroups.Count; i++)
            {
                var existing = existingGroups[i];
                var newGroup = newGroups.FirstOrDefault(g => g.CustomizationName == existing.CustomizationName);

                if (newGroup == null)
                    return true;

                // Check group properties
                if (existing.CustomizationName != newGroup.CustomizationName ||
                    existing.IsRequired != newGroup.IsRequired ||
                    existing.NumberOfChoice != newGroup.NumberOfChoice)
                    return true;

                // Check options count
                var existingOptions = existing.CustomizationOptions?.ToList() ?? new List<CustomizationOption>();
                var newOptions = newGroup.CustomizationOptions ?? new List<CustomizationOptionViewModel>();

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