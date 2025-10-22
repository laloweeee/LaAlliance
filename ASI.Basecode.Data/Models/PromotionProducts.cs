#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    public partial class PromotionProducts
    {
        public int PromotionID { get; set; }
        public int? ProductID { get; set; }

        //Navigator
        public RestaurantPromotions? RestaurantPromotion { get; set; } // One promotional product belongs to one promotion
        public Product? Product { get; set; } // One promotional product is associated with one product
    }
}
