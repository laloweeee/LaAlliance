using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        public int CategoryID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedTime { get; set; }

        // Navigator
        public User CreatedByUser { get; set; }
        public User UpdatedByUser { get; set; }
        public Category Category { get; set; }

        // Collection
        public ICollection<CustomizationGroup> CustomizationGroups { get; set; } = new List<CustomizationGroup>();
    }
}
