using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public int RestaurantId { get; set; }
        public Decimal SubTotal { get; set; }
        public Decimal Total { get; set; }
        public Decimal DeliveryFee { get; set; }
        public Decimal DiscountAmount { get; set; }
        
        // TODO: public int PromotionId { get; set; }

        public string OrderType { get; set; }
        public string OderStatus { get; set; }
        public string paymentStatus { get; set; }
        public DateTime CreatedTime { get; set; } // Timestamp of creation
        public DateTime UpdatedTime { get; set; } // Timestamp of last update
        public TimeOnly ElapsedTime { get; set; }
        public int ManagedBy { get; set; } // UserID of the handling staff
    }
}
