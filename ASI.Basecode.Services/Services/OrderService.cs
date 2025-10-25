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
using ASI.Basecode.Resources.Constants;

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
        private readonly IPaymentLogRepository _paymentLogRepository;
        private readonly IHubContext<OrderHub> _orderHubContext;
        private readonly IStaffRepository _staffRepository;

        private readonly IOrderProcessedRepository _orderProcessedRepository;

        public OrderService(
            IOrderRepository orderRepository,
            ICartRepository cartRepository,
            IAddressService addressService,
            IUserRepository userRepository,
            IOrderProcessedRepository orderProcessedRepository,
            IHubContext<OrderHub> orderHubContext,
            IPaymentLogRepository paymentLogRepository,
            IStaffRepository staffRepository
            )
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _addressService = addressService;
            _userRepository = userRepository;
            _orderProcessedRepository = orderProcessedRepository;
            _orderHubContext = orderHubContext;
            _paymentLogRepository = paymentLogRepository;
            _staffRepository = staffRepository;
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
            if (request.OrderType == OrderType.Delivery)
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

            decimal deliveryFee = request.OrderType == OrderType.Delivery ? 50.00m : 0.00m;
            decimal discountAmount = 0.00m;
            decimal totalAmount = subtotal + deliveryFee - discountAmount;

            Console.WriteLine($"Order Placed with payment method: {request.PaymentMethod}");

            // Create the order
            var order = new Order
            {
                UserID = request.UserID,
                OrderAddressID = addressId,
                OrderDate = DateTime.UtcNow,
                SubTotal = subtotal,
                DiscountAmount = discountAmount,
                TotalAmount = totalAmount,
                OrderType = request.OrderType,
                OrderStatus = OrderStatus.Pending,
                PaymentMethod = request.PaymentMethod,
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
                PaymentMethod = request.PaymentMethod,
                TransactionReference = request.PaymentMethod != PaymentMethod.CashOnDelivery ? GenerateTransactionReference() : "N/A",
                PaymentAmount = totalAmount,
                PaymentDate = DateTime.UtcNow,
                Remarks = "No Remarks",
                CreatedAt = DateTime.UtcNow
            };

            // Set payment status based on payment method
            if (request.PaymentMethod == PaymentMethod.CashOnDelivery && order.OrderType == OrderType.Delivery)
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
            await NotifyDashboardUpdate();

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
                    UnitPrice = oi.UnitPrice,
                    // ADD THIS SECTION TO INCLUDE OPTIONS:
                    OrderItemOptions = oi.OrderItemOption?.Select(oio => new OrderItemOptionViewModel
                    {
                        OrderItemOptionID = oio.OrderItemOptionID,
                        OptionGroupName = oio.ProductOptionGroup?.OptionGroupName ?? "",
                        OptionName = oio.ProductOptionItems?.OptionName ?? "",
                        AdditionalPrice = oio.ProductOptionItems?.AdditionalPrice ?? 0
                    }).ToList() ?? new List<OrderItemOptionViewModel>()
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
                    UnitPrice = oi.UnitPrice,
                    // ADD THIS SECTION TO INCLUDE OPTIONS:
                    OrderItemOptions = oi.OrderItemOption?.Select(oio => new OrderItemOptionViewModel
                    {
                        OrderItemOptionID = oio.OrderItemOptionID,
                        OptionGroupName = oio.ProductOptionGroup?.OptionGroupName ?? "",
                        OptionName = oio.ProductOptionItems?.OptionName ?? "",
                        AdditionalPrice = oio.ProductOptionItems?.AdditionalPrice ?? 0
                    }).ToList() ?? new List<OrderItemOptionViewModel>()
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

            // Update order status FIRST
            order.OrderStatus = newStatus;
            _orderRepository.UpdateOrder(order);
            
            Console.WriteLine($"✓ Order {orderID} status updated to {newStatus}");

            // Add OrderProcessed record when order is accepted
            if (newStatus == OrderStatus.Processing)
            {
                Console.WriteLine($"→ Attempting to create OrderProcessed for Order {orderID}");

                try
                {
                    // Get the StaffID from RestaurantStaff table using the UserID
                    var staff = user.RestaurantStaff; // Assuming User has RestaurantStaff navigation property

                    if (staff == null)
                    {
                        Console.WriteLine($"✗ No RestaurantStaff found for UserID {userID}");
                        // Don't fail the entire operation
                        return;
                    }

                    Console.WriteLine($"→ Found StaffID: {staff.StaffID} for UserID: {userID}");

                    // Check if record already exists
                    var existingProcessed = _orderProcessedRepository.GetAllOrderProcesseds()
                        .FirstOrDefault(op => op.OrderID == orderID);

                    if (existingProcessed == null)
                    {
                        Console.WriteLine("→ Creating new OrderProcessed record...");

                        var orderProcessed = new OrderProcessed
                        {
                            OrderID = orderID,
                            UserID = staff.StaffID,  // ← CHANGED: Use StaffID instead of UserID
                            ProcessedAt = DateTime.UtcNow,
                            ElapsedTime = TimeOnly.FromDateTime(DateTime.UtcNow)
                        };

                        _orderProcessedRepository.AddOrderProcessed(orderProcessed);

                        Console.WriteLine($"✓ OrderProcessed created successfully for OrderID: {orderID}");
                    }
                    else
                    {
                        Console.WriteLine("→ Updating existing OrderProcessed record...");

                        existingProcessed.UserID = staff.StaffID;  // ← CHANGED: Use StaffID
                        existingProcessed.ProcessedAt = DateTime.UtcNow;
                        existingProcessed.ElapsedTime = TimeOnly.FromDateTime(DateTime.UtcNow);

                        _orderProcessedRepository.UpdateOrderProcessed(existingProcessed);

                        Console.WriteLine($"✓ OrderProcessed updated for OrderID: {orderID}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"✗✗✗ ERROR saving OrderProcessed ✗✗✗");
                    Console.WriteLine($"Error Message: {ex.Message}");
                    Console.WriteLine($"Error Type: {ex.GetType().Name}");
                    Console.WriteLine($"Stack Trace: {ex.StackTrace}");

                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                    }
                }
            }

            if (newStatus == OrderStatus.Completed && order.PaymentMethod == PaymentMethod.CashOnDelivery)
            {
                var paymentLog = _paymentLogRepository.GetPaymentLogsByOrderId(orderID).AsQueryable().FirstOrDefault();
                
                if (paymentLog == null)
                {
                    Console.WriteLine($"✗ Payment log not found for Order {orderID}");
                    return;
                }

                paymentLog.PaymentStatus = PaymentStatus.Completed;
                _paymentLogRepository.UpdatePaymentLog(paymentLog);
            }

            // Notifications
            try
            {
                await NotifyOrderStatusChange(orderID, newStatus);
                await NotifyDashboardUpdate(); 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error notifying customer: {ex.Message}");
            }
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
                // Get the order to find user ID
                var order = _orderRepository.GetOrderById(orderID);
                if (order == null) return;

                var statusMessage = GetStatusMessage(newStatus);

                // Notify order-specific group (for individual order tracking pages)
                await _orderHubContext.Clients.Group($"Order-{orderID}")
                    .SendAsync("OrderStatusUpdated", new
                    {
                        orderId = orderID,
                        status = newStatus.ToString(),
                        statusMessage = statusMessage,
                        timestamp = DateTime.UtcNow
                    });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending status update notification: {ex.Message}");
            }
        }

        public async Task<DashboardStatsViewModel> GetDashboardStatsAsync()
        {
            try
            {
                var orders = _orderRepository.GetAllOrders();
                var today = DateTime.Today;
                
                // Add null checks
                if (orders == null)
                {
                    return new DashboardStatsViewModel();
                }
                
                var todaysOrders = orders.Count(o => o.OrderDate.Date == today);
                var totalOrders = orders.Count();
                
                // Get unique customers
                var todaysCustomers = orders
                    .Where(o => o.OrderDate.Date == today)
                    .Select(o => o.UserID)
                    .Where(userId => userId.HasValue)
                    .Distinct()
                    .Count();
                    
                var totalCustomers = orders
                    .Select(o => o.UserID)
                    .Where(userId => userId.HasValue)
                    .Distinct()
                    .Count();

                var totalSales = await GetTotalSalesAsync();
                var averageSalePerDay = await GetAverageSalePerDayAsync();
                var averageProcessingTime = await GetAverageProcessingTimeAsync(); 

                return new DashboardStatsViewModel
                {
                    TodaysOrders = todaysOrders,
                    TotalOrders = totalOrders,
                    TodaysCustomers = todaysCustomers,
                    TotalCustomers = totalCustomers,
                    TotalSales = totalSales,
                    AverageSalePerDay = averageSalePerDay,
                    AverageProcessingTime = averageProcessingTime
                };
            }
            catch (Exception ex)
            {
                // Log the error and return empty stats
                Console.WriteLine($"Error getting dashboard stats: {ex.Message}");
                return new DashboardStatsViewModel();
            }
        }

        public async Task<OrderSummaryViewModel> GetOrderSummaryAsync()
        {
            try
            {
                var orders = _orderRepository.GetAllOrders();
                
                if (orders == null)
                {
                    return new OrderSummaryViewModel();
                }
                
                return new OrderSummaryViewModel
                {
                    Pending = orders.Count(o => o.OrderStatus == OrderStatus.Pending),
                    Processing = orders.Count(o => o.OrderStatus == OrderStatus.Processing),
                    ReadyForDeliveryPickup = orders.Count(o => o.OrderStatus == OrderStatus.ReadyForDelivery || o.OrderStatus == OrderStatus.ReadyForPickup),
                    Completed = orders.Count(o => o.OrderStatus == OrderStatus.Completed),
                    Cancelled = orders.Count(o => o.OrderStatus == OrderStatus.Cancelled)
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting order summary: {ex.Message}");
                return new OrderSummaryViewModel();
            }
        }

        public async Task<decimal> GetTotalSalesAsync()
        {
            try
            {
                var orders = _orderRepository.GetAllOrders();
                if (orders == null || !orders.Any())
                    return 0;

                return orders
                    .Where(o => o.OrderStatus != OrderStatus.Cancelled)
                    .Sum(o => o.TotalAmount);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting total sales: {ex.Message}");
                return 0;
            }
        }

        public async Task<decimal> GetAverageSalePerDayAsync()
        {
            try
            {
                var orders = _orderRepository.GetAllOrders();
                if (orders == null || !orders.Any())
                    return 0;

                var completedOrders = orders.Where(o => o.OrderStatus != OrderStatus.Cancelled);
                
                if (!completedOrders.Any())
                    return 0;

                var orderDates = completedOrders.Select(o => o.OrderDate.Date).Distinct();
                var totalDays = orderDates.Count();
                
                if (totalDays == 0) 
                    return 0;
                
                return completedOrders.Sum(o => o.TotalAmount) / totalDays;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting average sales: {ex.Message}");
                return 0;
            }
        }
        
        private async Task NotifyDashboardUpdate()
        {
            try
            {
                var stats = await GetDashboardStatsAsync();
                var orderSummary = await GetOrderSummaryAsync();
                
                await _orderHubContext.Clients.Group("Dashboard")
                    .SendAsync("DashboardUpdated", new
                    {
                        stats = new
                        {
                            todaysOrders = stats.TodaysOrders,
                            totalOrders = stats.TotalOrders,
                            todaysCustomers = stats.TodaysCustomers,
                            totalCustomers = stats.TotalCustomers,
                            totalSales = stats.TotalSales,
                            averageSalePerDay = stats.AverageSalePerDay
                        },
                        orderSummary = new
                        {
                            pending = orderSummary.Pending,
                            processing = orderSummary.Processing,
                            readyForDeliveryPickup = orderSummary.ReadyForDeliveryPickup,
                            completed = orderSummary.Completed,
                            cancelled = orderSummary.Cancelled
                        },
                        timestamp = DateTime.UtcNow
                    });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending dashboard update: {ex.Message}");
            }
        }

        public async Task<List<StaffActivityViewModel>> GetRecentStaffActivitiesAsync(int count = 10)
        {
            try
            {
                var orderProcesseds = _orderProcessedRepository.GetAllOrderProcesseds()
                    .OrderByDescending(op => op.ProcessedAt)
                    .Take(count)
                    .ToList();

                var staffActivities = new List<StaffActivityViewModel>();

                foreach (var processed in orderProcesseds)
                {
                    // Manually get staff information using the UserID (which is actually StaffID)
                    string staffName = "Unassigned";
                    if (processed.UserID.HasValue)
                    {
                        var staff = _staffRepository.GetStaffByID(processed.UserID.Value);
                        if (staff != null && staff.User != null)
                        {
                            var userProfile = staff.User.UserProfile;
                            staffName = $"{userProfile?.FirstName} {userProfile?.LastName}".Trim();
                            
                            if (string.IsNullOrEmpty(staffName))
                            {
                                staffName = staff.User.Email ?? "Unknown Staff";
                            }
                        }
                    }

                    // Get customer name
                    string customerName = "Unknown Customer";
                    if (processed.Order?.User?.UserProfile != null)
                    {
                        var customerProfile = processed.Order.User.UserProfile;
                        customerName = $"{customerProfile.FirstName} {customerProfile.LastName}".Trim();
                    }

                    var staffActivity = new StaffActivityViewModel
                    {
                        OrderID = processed.OrderID,
                        StaffName = staffName,
                        ElapsedTime = processed.ElapsedTime.ToString(@"hh\:mm\:ss"),
                        ProcessedAt = processed.ProcessedAt,
                        CustomerName = customerName,
                        OrderType = processed.Order?.OrderType.ToString() ?? "Unknown"
                    };

                    staffActivities.Add(staffActivity);
                }

                return staffActivities;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting staff activities: {ex.Message}");
                return new List<StaffActivityViewModel>();
            }
        }

        public async Task<string> GetAverageProcessingTimeAsync()
        {
            try
            {
                var orderProcesseds = _orderProcessedRepository.GetAllOrderProcesseds().ToList();

                if (!orderProcesseds.Any())
                    return "00:00:00";

                // Calculate average elapsed time
                var totalSeconds = orderProcesseds.Average(op => 
                    op.ElapsedTime.Hour * 3600 + 
                    op.ElapsedTime.Minute * 60 + 
                    op.ElapsedTime.Second);

                var averageTimeSpan = TimeSpan.FromSeconds(totalSeconds);
                return averageTimeSpan.ToString(@"hh\:mm\:ss");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calculating average processing time: {ex.Message}");
                return "00:00:00";
            }
        }

        /// <summary>
        /// Notify all restaurant staff about order status change
        /// </summary>
        private async Task NotifyRestaurantOrderStatusChange(int orderID, OrderStatus newStatus)
        {
            /*try
            {
                await _orderHubContext.Clients.Group("Restaurant")
                    .SendAsync("OrderStatusChanged", new
                // Also notify user's orders group (for order activity page)
                await _orderHubContext.Clients.Group($"UserOrders-{order.UserID}")
                    .SendAsync("OrderStatusUpdated", new
                    {
                        orderId = orderID,
                        status = newStatus.ToString(),
                        statusMessage = statusMessage,
                        timestamp = DateTime.UtcNow
                    });

                Console.WriteLine($"✓ Sent real-time update for order {orderID} to user {order.UserID}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error sending status update notification: {ex.Message}");
            }*/
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
                OrderStatus.ReadyForPickup => "Your order is ready for pickup",
                OrderStatus.ReadyForDelivery => "Your order is ready for delivery", // ADD THIS
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
