using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class Category
    {
        public int CategoryID { get; set; }
        public string Name { get; set; }
        public int CreatedBy { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedTime { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedTime { get; set; }

        // Navigator
        public User CreatedByUser { get; set; }
        public User UpdatedByUser { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
