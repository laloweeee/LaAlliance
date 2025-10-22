#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Data.Models
{
    public partial class Order
    {
        public int OrderID { get; set; }
        public int? UserID { get; set; }
        public int? PromotionID { get; set; }
        public int? OrderAddressID { get; set; }

        public DateTime OrderDate { get; set; }
        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }

        public OrderType OrderType { get; set; } = OrderType.Delivery;
        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
        public PaymentMethod PaymentMethod { get; set; }

        //Navigator
        public User? User { get; set; } // One order is placed by one user
        public Address? Address { get; set; } // One order has one address
        public OrderProcessed OrderProcessed { get; set; } = null!; // One order has one order processed
        public RestaurantPromotions RestaurantPromotion { get; set; } = null!; // One order can have one promotion
        public ICollection<OrderItems> OrderItems { get; set; } = new List<OrderItems>(); // One order can have multiple order items
        public ICollection<PaymentLog> PaymentLogs { get; set; } = new List<PaymentLog>(); // One Order → Many Payments
    }
}
