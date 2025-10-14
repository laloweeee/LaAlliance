using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace ASI.Basecode.Services.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        public CartService(ICartRepository cartRepository, IProductService productService, IMapper mapper)
        {
            _cartRepository = cartRepository;
            _productService = productService;
            _mapper = mapper;
        }

        private Cart CreateCart(int userID)
        {
            var newCart = new Cart
            {
                UserID = userID,
                CreatedAt = DateTime.UtcNow
            };

            _cartRepository.AddCart(newCart);
            return newCart;
        }

        public CartViewModel GetOrCreateCart(int userID)
        {
            var cart = _cartRepository.GetCartByUserID(userID);
            if (cart == null)
            {
                cart = CreateCart(userID);
            }

            return MapCartToViewModel(cart);
        }

        public void AddItemToCart(AddToCartRequest request, int userID)
        {
            var cart = _cartRepository.GetCartByUserID(userID);
            if (cart == null)
            {
                cart = CreateCart(userID);
                // Refresh to get the cart with all relationships
                cart = _cartRepository.GetCartByUserID(userID);
            }

            // Get product details (without tracking)
            var product = _productService.GetProductByID(request.ProductID).FirstOrDefault();
            if (product == null) throw new Exception("Product not found");

            // Check if item with same options exists
            var existingCartItem = FindMatchingCartItem(cart.CartID, request.ProductID, request.SelectedOptions);

            if (existingCartItem != null)
            {
                // Update quantity of existing item
                existingCartItem.Quantity += request.Quantity;
                UpdateCartItemQuantity(existingCartItem.CartItemID, existingCartItem.Quantity, userID);
            }
            else
            {
                // Create completely new cart item
                var newCartItem = new CartItem
                {
                    CartID = cart.CartID,
                    ProductID = request.ProductID,
                    Quantity = request.Quantity,
                    UnitPrice = product.ProductPrice
                };

                // Save the cart item first to get the ID
                _cartRepository.AddCartItem(newCartItem);

                // Add the options if any exist
                if (request.SelectedOptions != null && request.SelectedOptions.Any())
                {
                    _cartRepository.AddCartItemOptions(newCartItem.CartItemID, request.SelectedOptions);
                }
            }
        }

        public void UpdateCartItemQuantity(int cartItemID, int quantity, int userID)
        {
            var cartItem = _cartRepository.GetCartItemByID(cartItemID);
            if (cartItem == null || cartItem.Cart.UserID != userID)
            {
                throw new Exception("Cart item not found");
            }

            if (quantity <= 0)
            {
                RemoveCartItem(cartItemID, userID);
            }
            else
            {
                cartItem.Quantity = quantity;
                _cartRepository.UpdateCartItem(cartItem);
            }
        }

        public void RemoveCartItem(int cartItemID, int userID)
        {
            var cartItem = _cartRepository.GetCartItemByID(cartItemID);
            if (cartItem == null || cartItem.Cart.UserID != userID)
            {
                throw new Exception("Cart item not found");
            }

            _cartRepository.RemoveCartItem(cartItem);
        }

        public void ClearCart(int userID)
        {
            _cartRepository.ClearCart(userID);
        }

        public CartItemViewModel GetCartItemForEdit(int cartItemID, int userID)
        {
            var cartItem = _cartRepository.GetCartItemByID(cartItemID);
            if (cartItem == null || cartItem.Cart.UserID != userID)
            {
                throw new Exception("Cart item not found");
            }

            return MapCartItemToViewModel(cartItem);
        }

        private CartItem FindMatchingCartItem(int cartID, int productID, Dictionary<int, List<int>> selectedOptions)
        {
            // Get all cart items for this product in this cart WITH OPTIONS LOADED
            var possibleMatches = _cartRepository.GetCartItems()
                .Where(ci => ci.CartID == cartID && ci.ProductID == productID)
                .ToList();

            // If no selected options, find items with no options
            if (selectedOptions == null || !selectedOptions.Any())
            {
                return possibleMatches.FirstOrDefault(ci => !ci.CartItemOption.Any());
            }

            // Find item with exact matching options
            foreach (var cartItem in possibleMatches)
            {
                // Ensure options are loaded
                if (cartItem.CartItemOption == null || !cartItem.CartItemOption.Any())
                    continue;

                if (OptionsMatch(cartItem.CartItemOption, selectedOptions))
                {
                    return cartItem;
                }
            }

            return null;
        }

        private bool OptionsMatch(ICollection<CartItemOption> existingOptions, Dictionary<int, List<int>> newOptions)
        {
            // If existing options is null or empty, they don't match unless newOptions is also empty
            if (existingOptions == null || !existingOptions.Any())
            {
                return newOptions == null || !newOptions.Any();
            }

            // Group existing options by ProductOptionGroupID
            var existingDict = existingOptions
                .GroupBy(o => o.ProductOptionGroupID)
                .ToDictionary(g => g.Key, g => g.Select(o => o.ProductOptionItemID).OrderBy(id => id).ToList());

            // Sort new options for comparison
            var newDict = newOptions
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.OrderBy(id => id).ToList());

            // Must have same number of option groups
            if (existingDict.Count != newDict.Count) return false;

            // Check each group
            foreach (var kvp in existingDict)
            {
                // New options must contain this group
                if (!newDict.ContainsKey(kvp.Key)) return false;

                // The option item IDs must match exactly
                if (!kvp.Value.SequenceEqual(newDict[kvp.Key])) return false;
            }

            return true;
        }

        private CartViewModel MapCartToViewModel(Cart cart)
        {
            return new CartViewModel
            {
                CartID = cart.CartID,
                UserID = cart.UserID,
                CartItems = cart.CartItem?.Select(MapCartItemToViewModel).ToList() ?? new List<CartItemViewModel>()
            };
        }

        private CartItemViewModel MapCartItemToViewModel(CartItem cartItem)
        {
            return new CartItemViewModel
            {
                CartItemID = cartItem.CartItemID,
                ProductID = cartItem.ProductID,
                ProductName = cartItem.Product.ProductName,
                ProductImage = cartItem.Product.ProductImage,
                Quantity = cartItem.Quantity,
                UnitPrice = cartItem.UnitPrice,
                Options = cartItem.CartItemOption?.Select(o => new CartItemOptionViewModel
                {
                    CartItemOptionID = o.CartItemOptionID,
                    ProductOptionGroupID = o.ProductOptionGroupID,
                    ProductOptionItemID = o.ProductOptionItemID,
                    OptionGroupName = o.ProductOptionGroup?.OptionGroupName,
                    OptionName = o.ProductOptionItems?.OptionName,
                    AdditionalPrice = o.ProductOptionItems?.AdditionalPrice ?? 0
                }).ToList() ?? new List<CartItemOptionViewModel>()
            };
        }
    }
}