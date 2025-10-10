using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    public partial class RestaurantPromotions
    {
        public int PromotionID { get; set; }
        public int RestaurantID { get; set; }
        public string PromotionName { get; set; }
        public string PromotionBanner { get; set; }
        public string PromotionDescription { get; set; }
        public string DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal MinimumOrderAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }

        //Navigator
        public Restaurant Restaurant { get; set; }
        public ICollection<PromotionProducts> PromotionProducts { get; set; } = [];
        public ICollection<PromotionCodes> PromotionCodes { get; set; } = [];
        public ICollection<Order> Order { get; set; } = [];
    }
}   
