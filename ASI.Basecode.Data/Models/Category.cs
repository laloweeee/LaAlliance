using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    public class Category
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

        // Collection
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
