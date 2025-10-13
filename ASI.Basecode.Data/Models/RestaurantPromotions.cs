using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Data.Models
{
    public partial class RestaurantPromotions
    {
        public int PromotionID { get; set; }
        public string PromotionName { get; set; }
        public string PromotionBanner { get; set; }
        public string PromotionDescription { get; set; }
        public DiscountType DiscountType { get; set; } = DiscountType.Percentage;
        public decimal DiscountValue { get; set; }
        public decimal MinimumOrderAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; } = true;

        //Navigator
        public PromotionCodes PromotionCodes { get; set; } // One promotion has one promotion code
        public ICollection<PromotionProducts> PromotionProducts { get; set; } = new List<PromotionProducts>(); // One promotion can have multiple products
        public ICollection<Order> Order { get; set; } = new List<Order>(); // One promotion can be applied to multiple orders
    }
}   
