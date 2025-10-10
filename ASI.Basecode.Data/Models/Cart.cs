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
        public int CustomerID { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool isCheckedOut { get; set; }

        //Navigator
        public User User { get; set; }
        public ICollection<CartItem> CartItem { get; set; } = [];
    }
}
