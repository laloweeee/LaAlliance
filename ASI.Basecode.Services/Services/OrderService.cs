using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.Services.Hubs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static ASI.Basecode.Resources.Constants.Enums;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Services
{
    /// <summary>
    /// Service for managing orders
    /// </summary>
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IAddressService _addressService;
        private readonly IUserRepository _userRepository;
        private readonly IHubContext<OrderHub> _orderHubContext;


        private readonly IOrderProcessedRepository _orderProcessedRepository;

        public OrderService(
            IOrderRepository orderRepository,
            ICartRepository cartRepository,
            IAddressService addressService,
            IUserRepository userRepository,
            IOrderProcessedRepository orderProcessedRepository,
            IHubContext<OrderHub> orderHubContext
            )
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _addressService = addressService;
            _userRepository = userRepository;
            _orderProcessedRepository = orderProcessedRepository;
            _orderHubContext = orderHubContext;
        }

        /// <summary>
        /// Places a new order based on the user's cart and checkout information
        /// </summary>
        /// <param name="request">Checkout request containing order details</param>
        /// <returns>OrderViewModel containing the created order details</returns>
        /// <exception cref="Exception"></exception>
        public async Task<OrderViewModel> PlaceOrder(PlaceOrderRequest request)
        {
            // Get user's cart
            var cart = _cartRepository.GetCartByUserID(request.UserID);
            if (cart == null || !cart.CartItem.Any())
            {
                throw new Exception("Cart is empty. Cannot place order.");
            }

            // Validate and get address for delivery orders
            int? addressId = null;
            if (Enum.Parse<OrderType>(request.OrderType) == OrderType.Delivery)
            {
                if (!request.SelectedAddressId.HasValue)
                {
                    throw new Exception("Delivery address is required for delivery orders.");
                }

                var userAddress = _addressService.GetUserAddresses(request.UserID)
                    .FirstOrDefault(a => a.UserAddressID == request.SelectedAddressId.Value);

                if (userAddress == null)
                {
                    throw new Exception("Selected address not found.");
                }

                addressId = userAddress.AddressID;
            }

            // Calculate order totals
            decimal subtotal = cart.CartItem.Sum(item =>
                item.Quantity * (item.UnitPrice + (item.CartItemOption?.Sum(opt => opt.ProductOptionItems?.AdditionalPrice ?? 0) ?? 0)));

            decimal deliveryFee = Enum.Parse<OrderType>(request.OrderType) == OrderType.Delivery ? 50.00m : 0.00m;
            decimal discountAmount = 0.00m; // TODO: Implement voucher/promotion logic
            decimal totalAmount = subtotal + deliveryFee - discountAmount;

            // Create the order
            var order = new Order
            {
                UserID = request.UserID,
                OrderAddressID = addressId,
                OrderDate = DateTime.UtcNow,
                SubTotal = subtotal,
                DiscountAmount = discountAmount,
                TotalAmount = totalAmount,
                OrderType = Enum.Parse<OrderType>(request.OrderType),
                OrderStatus = OrderStatus.Pending,
                PaymentMethod = Enum.Parse<PaymentMethod>(request.PaymentMethod),
                OrderItems = new List<OrderItems>()
            };

            // Create order items from cart items
            foreach (var cartItem in cart.CartItem)
            {
                var orderItem = new OrderItems
                {
                    ProductID = cartItem.ProductID,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.UnitPrice,
                    OrderItemOption = new List<OrderItemOption>()
                };

                // Create order item options from cart item options
                if (cartItem.CartItemOption != null && cartItem.CartItemOption.Any())
                {
                    foreach (var cartItemOption in cartItem.CartItemOption)
                    {
                        var orderItemOption = new OrderItemOption
                        {
                            ProductOptionGroupID = cartItemOption.ProductOptionGroupID,
                            ProductOptionItemID = cartItemOption.ProductOptionItemID
                        };
                        orderItem.OrderItemOption.Add(orderItemOption);
                    }
                }

                order.OrderItems.Add(orderItem);
            }

            // Create payment log
            var paymentLog = new PaymentLog
            {
                UserID = request.UserID,
                PaymentMethod = Enum.Parse<PaymentMethod>(request.PaymentMethod),
                TransactionReference = GenerateTransactionReference(),
                PaymentAmount = totalAmount,
                PaymentDate = DateTime.UtcNow,
                Remarks = request.DeliveryNotes ?? "No remarks",
                CreatedAt = DateTime.UtcNow
            };

            // Set payment status based on payment method
            if (request.PaymentMethod == "CashOnDelivery" && order.OrderType == OrderType.Delivery)
            {
                // Set payment status to pending for cash on delivery
                paymentLog.PaymentStatus = PaymentStatus.Pending;
            }
            else
            {
                // For other payment methods, assume payment is completed
                paymentLog.PaymentStatus = PaymentStatus.Completed;
            }

            // Add payment log to order
            order.PaymentLogs.Add(paymentLog);

            // Save the order
            _orderRepository.AddOrder(order);

            // Clear the cart after successful order
            _cartRepository.ClearCart(request.UserID);

            var orderViewModel = GetOrderById(order.OrderID);
            // Notify restaurant of new order via SignalR        
            await NotifyRestaurantNewOrder(orderViewModel);

            // Return order view model
            return orderViewModel;
        }

        /// <summary>
        /// Gets order details by order ID
        /// </summary>
        /// <param name="orderID">Order ID</param>
        /// <returns>OrderViewModel containing order details</returns>
        public OrderViewModel GetOrderById(int orderID)
        {
            var order = _orderRepository.GetOrderById(orderID);
            if (order == null)
            {
                throw new Exception("Order not found.");
            }

            var orderViewModel = new OrderViewModel
            {
                OrderID = order.OrderID,
                UserID = order.UserID ?? 0,
                OrderDate = order.OrderDate,
                SubTotal = order.SubTotal,
                DiscountAmount = order.DiscountAmount,
                DeliveryFee = order.OrderType == OrderType.Delivery ? 50.00m : 0.00m,
                TotalAmount = order.TotalAmount,
                OrderType = order.OrderType,
                OrderStatus = order.OrderStatus,
                PaymentMethod = order.PaymentMethod,
                FirstName = order.User?.UserProfile?.FirstName ?? "",
                LastName = order.User?.UserProfile?.LastName ?? "",
                ContactNumber = order.User?.UserProfile?.ContactNumber ?? "",
                OrderItems = order.OrderItems.Select(oi => new OrderItemViewModel
                {
                    OrderItemID = oi.OderItemID,
                    ProductID = oi.ProductID ?? 0,
                    ProductName = oi.Product?.ProductName ?? "Unknown Product",
                    ProductImage = oi.Product?.ProductImage ?? "",
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    OrderItemOptions = oi.OrderItemOption.Select(oio => new OrderItemOptionViewModel
                    {
                        OrderItemOptionID = oio.OrderItemOptionID,
                        OptionGroupName = oio.ProductOptionGroup?.OptionGroupName ?? "",
                        OptionName = oio.ProductOptionItems?.OptionName ?? "",
                        AdditionalPrice = oio.ProductOptionItems?.AdditionalPrice ?? 0
                    }).ToList()
                }).ToList()
            };

            // Get delivery address if it's a delivery order
            if (order.OrderType == OrderType.Delivery && order.OrderAddressID.HasValue)
            {
                var address = order.Address;
                if (address != null)
                {
                    orderViewModel.DeliveryAddress = new DeliveryAddressViewModel
                    {
                        AddressID = address.AddressID,
                        Longitude = address.Longitude,
                        Latitude = address.Latitude,
                        Street = address.Street,
                        Barangay = address.Barangay,
                        City = address.City,
                        Province = address.Province,
                        ZipCode = address.ZipCode,
                        Country = address.Country
                    };
                }
            }

            return orderViewModel;
        }

        /// <summary>
        /// Gets all orders for a specific user
        /// </summary>
        /// <param name="userID">User ID</param>
        /// <returns>List of OrderViewModel</returns>
        public IEnumerable<OrderViewModel> GetOrdersByUserId(int userID)
        {
            var orders = _orderRepository.GetAllOrders()
                .Where(o => o.UserID == userID).ToList();

            return orders.Select(order => new OrderViewModel
            {
                OrderID = order.OrderID,
                UserID = order.UserID ?? 0,
                OrderDate = order.OrderDate,
                SubTotal = order.SubTotal,
                DiscountAmount = order.DiscountAmount,
                DeliveryFee = order.OrderType == OrderType.Delivery ? 50.00m : 0.00m,
                TotalAmount = order.TotalAmount,
                OrderType = order.OrderType,
                OrderStatus = order.OrderStatus,
                PaymentMethod = order.PaymentMethod,
                FirstName = order.User?.UserProfile?.FirstName ?? "",
                LastName = order.User?.UserProfile?.LastName ?? "",
                ContactNumber = order.User?.UserProfile?.ContactNumber ?? "",
                OrderItems = order.OrderItems.Select(oi => new OrderItemViewModel
                {
                    OrderItemID = oi.OderItemID,
                    ProductID = oi.ProductID ?? 0,
                    ProductName = oi.Product?.ProductName ?? "Unknown Product",
                    ProductImage = oi.Product?.ProductImage ?? "",
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            }).ToList();
        }

        /// <summary>
        /// Gets all orders
        /// </summary>
        /// <returns>List of OrderViewModel</returns>
        public List<OrderViewModel> GetAllOrders()
        {
            var orders = _orderRepository.GetAllOrders();
            return orders.Select(order => new OrderViewModel
            {
                OrderID = order.OrderID,
                UserID = order.UserID ?? 0,
                OrderDate = order.OrderDate,
                SubTotal = order.SubTotal,
                DiscountAmount = order.DiscountAmount,
                DeliveryFee = order.OrderType == OrderType.Delivery ? 50.00m : 0.00m,
                TotalAmount = order.TotalAmount,
                OrderType = order.OrderType,
                OrderStatus = order.OrderStatus,
                PaymentMethod = order.PaymentMethod,
                FirstName = order.User?.UserProfile?.FirstName ?? "",
                LastName = order.User?.UserProfile?.LastName ?? "",
                ContactNumber = order.User?.UserProfile?.ContactNumber ?? "",
                OrderItems = order.OrderItems.Select(oi => new OrderItemViewModel
                {
                    OrderItemID = oi.OderItemID,
                    ProductID = oi.ProductID ?? 0,
                    ProductName = oi.Product?.ProductName ?? "Unknown Product",
                    ProductImage = oi.Product?.ProductImage ?? "",
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            }).ToList();
        }

        /// <summary>
        /// Cancels an order
        /// </summary>
        /// <param name="orderID">Order ID</param>
        /// <param name="userId">User ID</param>
        public void CancelOrder(int orderID, int userId)
        {
            var order = _orderRepository.GetAllOrders().FirstOrDefault(o => o.OrderID == orderID);
            if (order == null)
            {
                throw new Exception("Order not found.");
            }

            if (order.UserID != userId)
            {
                throw new Exception("You are not authorized to cancel this order.");
            }

            if (order.OrderStatus != OrderStatus.Pending)
            {
                throw new Exception("Only pending orders can be cancelled.");
            }

            order.OrderStatus = OrderStatus.Cancelled;
            _orderRepository.UpdateOrder(order);
        }

        /// <summary>
        /// Restaurant updates an order status
        /// Change from new to processing to out for delivery or ready for pickup
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="userID"></param>
        /// <param name="newStatus"></param>
        /// <exception cref="InvalidDataException"></exception>
        public async Task UpdateOrderStatus(int orderID, int userID, OrderStatus newStatus)
        {
            var order = _orderRepository.GetOrderById(orderID);

            if (order == null)
            {
                throw new InvalidDataException("Order not found.");
            }

            var user = _userRepository.GetUserByID(userID);

            if (user == null)
            {
                throw new InvalidDataException("User not found.");
            }

            if (!user.UserType.Equals(Enum.Parse<UserType>("Restaurant")))
            {
                throw new InvalidDataException("User is not a restaurant.");
            }

            if (newStatus == OrderStatus.Pending)
            {
                _orderProcessedRepository.AddOrderProcessed(new OrderProcessed
                {
                    OrderID = orderID,
                    UserID = userID,
                    ProcessedAt = DateTime.UtcNow,
                });
            }

            // Update order status
            _orderRepository.UpdateOrder(new Order
            {
                OrderID = orderID,
                OrderStatus = newStatus
            });

            // Notify customer about order status change
            await NotifyOrderStatusChange(orderID, newStatus);
        }








        /// <summary>
        /// Notify restaurant of new order via SignalR
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private async Task NotifyRestaurantNewOrder(OrderViewModel order)
        {
            try
            {
                await _orderHubContext.Clients.Group("Restaurant")
                    .SendAsync("NewOrderReceived", new
                    {
                        orderId = order.OrderID,
                        customerName = $"{order.FirstName} {order.LastName}",
                        contactNumber = order.ContactNumber,
                        orderType = order.OrderType.ToString(),
                        paymentMethod = order.PaymentMethod.ToString(),
                        items = order.OrderItems.Select(item => new
                        {
                            name = item.ProductName,
                            image = item.ProductImage,
                            quantity = item.Quantity,
                            price = item.UnitPrice,
                            options = item.OrderItemOptions?.Select(opt => new
                            {
                                groupName = opt.OptionGroupName,
                                optionName = opt.OptionName,
                                additionalPrice = opt.AdditionalPrice
                            }).ToList()
                        }).ToList(),
                        subtotal = order.SubTotal,
                        deliveryFee = order.DeliveryFee,
                        discountAmount = order.DiscountAmount,
                        totalAmount = order.TotalAmount,
                        deliveryAddress = order.DeliveryAddress != null ? new
                        {
                            street = order.DeliveryAddress.Street,
                            barangay = order.DeliveryAddress.Barangay,
                            city = order.DeliveryAddress.City,
                            province = order.DeliveryAddress.Province,
                            zipCode = order.DeliveryAddress.ZipCode,
                            fullAddress = $"{order.DeliveryAddress.Street}, {order.DeliveryAddress.Barangay}, {order.DeliveryAddress.City}"
                        } : null,
                        timestamp = DateTime.UtcNow
                    });
            }
            catch (Exception ex)
            {
                // Log error but don't fail the order creation
                Console.WriteLine($"Error sending SignalR notification: {ex.Message}");
            }
        }

        /// <summary>
        /// Notify order status change via SignalR
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="newStatus"></param>
        /// <returns></returns>
        private async Task NotifyOrderStatusChange(int orderID, OrderStatus newStatus)
        {
            try
            {
                await _orderHubContext.Clients.Group($"Order-{orderID}")
                    .SendAsync("OrderStatusUpdated", new
                    {
                        orderId = orderID,
                        status = newStatus.ToString(),
                        statusMessage = GetStatusMessage(newStatus),
                        timestamp = DateTime.UtcNow
                    });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending status update notification: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets user-friendly status message
        /// </summary>
        private string GetStatusMessage(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Pending => "Your order has been received and is awaiting confirmation",
                OrderStatus.Processing => "Your order is being prepared",
                OrderStatus.OutForDelivery => "Your order is out for delivery",
                OrderStatus.ReadyForPickup => "Your order is ready for pickup",
                OrderStatus.Completed => "Your order has been completed",
                OrderStatus.Cancelled => "Your order has been cancelled",
                _ => "Order status updated"
            };
        }

        /// <summary>
        /// Generates a unique transaction reference
        /// </summary>
        /// <returns>Transaction reference string</returns>
        private string GenerateTransactionReference()
        {
            return $"TXN-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        }
    }
}
