using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    /// <summary>
    /// Promotion Codes entity with soft delete support
    /// </summary>
    public partial class PromotionCodes : BaseEntity
    {
        public int PromotionCodeID { get; set; }
        public int PromotionID { get; set; }
        public string Code { get; set; }
        public int UsageLimit { get; set; }
        public int UsedCount { get; set; }
        public DateTime ExpirationDate { get; set; }

        //Navigator
        public RestaurantPromotions RestaurantPromotions { get; set; } // One promotion has one promotion code
    }
}
