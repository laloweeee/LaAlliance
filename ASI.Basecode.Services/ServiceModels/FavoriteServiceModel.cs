using System;
using System.Collections.Generic;

namespace ASI.Basecode.Services.ServiceModels
{
    public class FavoriteServiceModel
    {
        public int UserID { get; set; }
        public int TotalItems => FavoriteItems?.Count ?? 0;
        public List<FavoriteItemServiceModel> FavoriteItems { get; set; } = new List<FavoriteItemServiceModel>();
    }

    public class FavoriteItemServiceModel
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