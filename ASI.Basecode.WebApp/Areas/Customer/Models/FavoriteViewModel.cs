using System;
using System.Collections.Generic;
using System.Linq;

namespace ASI.Basecode.WebApp.Areas.Customer.Models
{
    public class FavoriteViewModel
    {
        public int FavoriteID { get; set; }
        public int UserID { get; set; }
        public int TotalItems => FavoriteItems.Count;
        public List<FavoriteItemViewModel> FavoriteItems { get; set; } = new List<FavoriteItemViewModel>();
    }

    public class FavoriteItemViewModel
    {
        public int FavoriteItemID { get; set; }
        public int ProductID { get; set; }
        public string ProductImage { get; set; }
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public string ProductDescription { get; set; }
        public string CategoryName { get; set; }
        public DateTime DateAdded { get; set; }
    }
}