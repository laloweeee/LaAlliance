using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ASI.Basecode.Data.Models;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Services.ServiceModels
{
    /// <summary>
    /// View model for Order display
    /// </summary>
    public class OrderViewModel
    {
        public int OrderID { get; set; }
        public int UserID { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderType OrderType { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string DeliveryNotes { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ContactNumber { get; set; }
        public DeliveryAddressViewModel DeliveryAddress { get; set; }
        public string VoucherCode { get; set; }
        public List<OrderItemViewModel> OrderItems { get; set; } = new List<OrderItemViewModel>();
    }

    /// <summary>
    /// View model for Order Items
    /// </summary>
    public class OrderItemViewModel
    {
        public int OrderItemID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => Quantity * UnitPrice + OrderItemOptions.Sum(opt => opt.AdditionalPrice * Quantity);
        public List<OrderItemOptionViewModel> OrderItemOptions { get; set; } = new List<OrderItemOptionViewModel>();
    }

    /// <summary>
    /// View model for Order Item Options
    /// </summary>
    public class OrderItemOptionViewModel
    {
        public int OrderItemOptionID { get; set; }
        public string OptionGroupName { get; set; }
        public string OptionName { get; set; }
        public decimal AdditionalPrice { get; set; }
    }

    /// <summary>
    /// Request model for placing an order
    /// </summary>
    public class PlaceOrderRequest
    {
        public int UserID { get; set; }
        public int? SelectedAddressId { get; set; }
        public OrderType OrderType { get; set; }
        public PaymentMethod PaymentMethod { get; set; } 
        public string DeliveryNotes { get; set; }
        public string VoucherCode { get; set; }
    }

    /// <summary>
    /// View model for delivery address used in orders
    /// </summary>
    public class DeliveryAddressViewModel
    {
        public int AddressID { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string Street { get; set; }
        public string Barangay { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public int ZipCode { get; set; }
        public string Country { get; set; } = "Philippines";
        
        // Formatted full address
        public string FullAddress => $"{Street}, {Barangay}, {City}, {Province} {ZipCode}";
    }
}
