using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    public partial class Cart
    {
        public int CartID { get; set; }
        public int UserID { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        //Navigator
        public User User { get; set; } // One cart belongs to one user
        public ICollection<CartItem> CartItem { get; set; } = new List<CartItem>(); // One cart can have multiple cart items
    }
}
