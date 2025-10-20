#nullable enable
using Xunit;
using Moq;
using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASI.Basecode.Tests.Service
{
    public class CartServiceTest
    {
        private readonly Mock<ICartRepository> _cartRepoMock;
        private readonly Mock<IProductService> _productServiceMock;
        private readonly Mock<AutoMapper.IMapper> _mapperMock;
        private readonly CartService _cartService;

        public CartServiceTest()
        {
            _cartRepoMock = new Mock<ICartRepository>();
            _productServiceMock = new Mock<IProductService>();
            _mapperMock = new Mock<AutoMapper.IMapper>();
            _cartService = new CartService(_cartRepoMock.Object, _productServiceMock.Object, _mapperMock.Object);
        }

        [Fact]
        public void GetOrCreateCart_ReturnsExistingCart()
        {
            var cart = new Cart { CartID = 1, UserID = 2, CartItem = new List<CartItem>() };
            _cartRepoMock.Setup(r => r.GetCartByUserID(2)).Returns(cart);

            var result = _cartService.GetOrCreateCart(2);

            Assert.Equal(1, result.CartID);
            Assert.Equal(2, result.UserID);
        }

        [Fact]
        public void GetOrCreateCart_CreatesNewCartIfNotExists()
        {
            _cartRepoMock.Setup(r => r.GetCartByUserID(3)).Returns((Cart)null);
            _cartRepoMock.Setup(r => r.AddCart(It.IsAny<Cart>())).Callback<Cart>(c => c.CartID = 99);
            _cartRepoMock.Setup(r => r.GetCartByUserID(3)).Returns(new Cart { CartID = 99, UserID = 3 });

            var result = _cartService.GetOrCreateCart(3);

            Assert.Equal(99, result.CartID);
            Assert.Equal(3, result.UserID);
        }

        [Fact]
        public void AddItemToCart_AddsNewItemIfNotExists()
        {
            var cart = new Cart { CartID = 1, UserID = 2, CartItem = new List<CartItem>() };
            var product = new ProductViewModel { ProductID = 10, ProductPrice = 5.5m };
            var request = new AddToCartRequest { ProductID = 10, Quantity = 2, SelectedOptions = null };

            _cartRepoMock.Setup(r => r.GetCartByUserID(2)).Returns(cart);
            _productServiceMock.Setup(s => s.GetProductByID(10)).Returns(product);
            _cartRepoMock.Setup(r => r.GetCartItems()).Returns(new List<CartItem>().AsQueryable());
            _cartRepoMock.Setup(r => r.AddCartItem(It.IsAny<CartItem>())).Callback<CartItem>(ci => ci.CartItemID = 123);

            _cartService.AddItemToCart(request, 2);

            _cartRepoMock.Verify(r => r.AddCartItem(It.Is<CartItem>(ci => ci.ProductID == 10 && ci.Quantity == 2)), Times.Once);
        }

        [Fact]
        public void AddItemToCart_UpdatesQuantityIfItemExists()
        {
            var cart = new Cart { CartID = 1, UserID = 2, CartItem = new List<CartItem>() };
            var product = new ProductViewModel { ProductID = 10, ProductPrice = 5.5m };
            var request = new AddToCartRequest { ProductID = 10, Quantity = 2, SelectedOptions = null };
            var existingItem = new CartItem
            {
                CartItemID = 5,
                CartID = 1,
                ProductID = 10,
                Quantity = 1,
                Cart = cart, // Ensure Cart is set
                CartItemOption = new List<CartItemOption>()
            };

            _cartRepoMock.Setup(r => r.GetCartByUserID(2)).Returns(cart);
            _productServiceMock.Setup(s => s.GetProductByID(10)).Returns(product);
            _cartRepoMock.Setup(r => r.GetCartItems()).Returns(new List<CartItem> { existingItem }.AsQueryable());
            _cartRepoMock.Setup(r => r.GetCartItemByID(5)).Returns(existingItem);
            _cartRepoMock.Setup(r => r.UpdateCartItem(It.IsAny<CartItem>()));

            _cartService.AddItemToCart(request, 2);

            _cartRepoMock.Verify(r => r.UpdateCartItem(It.Is<CartItem>(ci => ci.CartItemID == 5 && ci.Quantity == 3)), Times.Once);
        }

        [Fact]
        public void AddItemToCart_ThrowsIfProductNotFound()
        {
            var cart = new Cart { CartID = 1, UserID = 2, CartItem = new List<CartItem>() };
            var request = new AddToCartRequest { ProductID = 99, Quantity = 1, SelectedOptions = null };

            _cartRepoMock.Setup(r => r.GetCartByUserID(2)).Returns(cart);
            _productServiceMock.Setup(s => s.GetProductByID(99)).Returns((ProductViewModel)null); // <-- Fix here

            Assert.Throws<Exception>(() => _cartService.AddItemToCart(request, 2));
        }

        [Fact]
        public void UpdateCartItemQuantity_UpdatesQuantity()
        {
            var cart = new Cart { CartID = 1, UserID = 2 };
            var cartItem = new CartItem { CartItemID = 10, Quantity = 1, Cart = cart };

            _cartRepoMock.Setup(r => r.GetCartItemByID(10)).Returns(cartItem);

            _cartService.UpdateCartItemQuantity(10, 5, 2);

            Assert.Equal(5, cartItem.Quantity);
            _cartRepoMock.Verify(r => r.UpdateCartItem(cartItem), Times.Once);
        }

        [Fact]
        public void UpdateCartItemQuantity_RemovesIfZero()
        {
            var cart = new Cart { CartID = 1, UserID = 2 };
            var cartItem = new CartItem { CartItemID = 10, Quantity = 1, Cart = cart };

            _cartRepoMock.Setup(r => r.GetCartItemByID(10)).Returns(cartItem);

            _cartService.UpdateCartItemQuantity(10, 0, 2);

            _cartRepoMock.Verify(r => r.RemoveCartItem(cartItem), Times.Once);
        }

        [Fact]
        public void UpdateCartItemQuantity_ThrowsIfNotFoundOrWrongUser()
        {
            _cartRepoMock.Setup(r => r.GetCartItemByID(10)).Returns((CartItem)null);
            Assert.Throws<Exception>(() => _cartService.UpdateCartItemQuantity(10, 1, 2));

            var cart = new Cart { CartID = 1, UserID = 99 };
            var cartItem = new CartItem { CartItemID = 10, Quantity = 1, Cart = cart };
            _cartRepoMock.Setup(r => r.GetCartItemByID(10)).Returns(cartItem);

            Assert.Throws<Exception>(() => _cartService.UpdateCartItemQuantity(10, 1, 2));
        }

        [Fact]
        public void RemoveCartItem_RemovesItem()
        {
            var cart = new Cart { CartID = 1, UserID = 2 };
            var cartItem = new CartItem { CartItemID = 10, Cart = cart };

            _cartRepoMock.Setup(r => r.GetCartItemByID(10)).Returns(cartItem);

            _cartService.RemoveCartItem(10, 2);

            _cartRepoMock.Verify(r => r.RemoveCartItem(cartItem), Times.Once);
        }

        [Fact]
        public void RemoveCartItem_ThrowsIfNotFoundOrWrongUser()
        {
            _cartRepoMock.Setup(r => r.GetCartItemByID(10)).Returns((CartItem)null);
            Assert.Throws<Exception>(() => _cartService.RemoveCartItem(10, 2));

            var cart = new Cart { CartID = 1, UserID = 99 };
            var cartItem = new CartItem { CartItemID = 10, Cart = cart };
            _cartRepoMock.Setup(r => r.GetCartItemByID(10)).Returns(cartItem);

            Assert.Throws<Exception>(() => _cartService.RemoveCartItem(10, 2));
        }

        [Fact]
        public void ClearCart_CallsRepository()
        {
            _cartService.ClearCart(2);
            _cartRepoMock.Verify(r => r.ClearCart(2), Times.Once);
        }

        [Fact]
        public void GetCartItemForEdit_ReturnsViewModel()
        {
            var cart = new Cart { CartID = 1, UserID = 2 };
            var product = new Product { ProductID = 10, ProductName = "Test", ProductImage = "img.png" };
            var cartItem = new CartItem
            {
                CartItemID = 10,
                Cart = cart,
                ProductID = 10,
                Product = product, // Use Product here
                Quantity = 1,
                UnitPrice = 5.5m,
                CartItemOption = new List<CartItemOption>()
            };

            _cartRepoMock.Setup(r => r.GetCartItemByID(10)).Returns(cartItem);

            var result = _cartService.GetCartItemForEdit(10, 2);

            Assert.Equal(10, result.CartItemID);
            Assert.Equal(10, result.ProductID);
            Assert.Equal("Test", result.ProductName); // Should now pass
        }

        [Fact]
        public void GetCartItemForEdit_ThrowsIfNotFoundOrWrongUser()
        {
            _cartRepoMock.Setup(r => r.GetCartItemByID(10)).Returns((CartItem)null);
            Assert.Throws<Exception>(() => _cartService.GetCartItemForEdit(10, 2));

            var cart = new Cart { CartID = 1, UserID = 99 };
            var cartItem = new CartItem { CartItemID = 10, Cart = cart };
            _cartRepoMock.Setup(r => r.GetCartItemByID(10)).Returns(cartItem);

            Assert.Throws<Exception>(() => _cartService.GetCartItemForEdit(10, 2));
        }
    }
}