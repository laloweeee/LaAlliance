using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    public partial class PromotionCodes
    {
        public int PromotionCodeID { get; set; }
        public int PromotionID { get; set; }
        public string Code { get; set; }
        public int UsageLimit { get; set; }
        public int UsedCount { get; set; }
        public DateTime ExpirationDate { get; set; }

        //Navigator
        public RestaurantPromotions RestaurantPromotions { get; set; }
    }
}
