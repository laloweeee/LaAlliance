using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    public partial class Order
    {
        public int OrderID { get; set; }
        public int UserID { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        
        // TODO: public int PromotionId { get; set; }

        public string OrderType { get; set; }
        public string OrderStatus { get; set; }
        public string OrderAddress { get; set; }
        public int PromotionID { get; set; }

        //Navigator
        public RestaurantPromotions RestaurantPromotions { get; set; }
        public User User { get; set; }
        public Address Address { get; set; }
        public ICollection<OrderItems> OrderItems { get; set; } = [];
        public ICollection<OrderProcessed> OrderProcessed { get; set; } = [];
    }
}
