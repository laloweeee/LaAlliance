using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    public partial class CustomerProductFavorites
    {
        public int CustomerProductFavoriteID { get; set; }
        public int UserID { get; set; }
        public int ProductID { get; set; }
        public DateTime DateAdded { get; set; }

        //Navigator
        public Product Product { get; set; } // One customer favorite is for one product
        public User User { get; set; } // One customer favorite is for one user
    }
}
