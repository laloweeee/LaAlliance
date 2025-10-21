using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASI.Basecode.Data.Models
{
    public class FavoriteItem
    {
        [Key]
        public int FavoriteItemID { get; set; }
        
        [Required]
        public int FavoriteID { get; set; }
        
        [Required]
        public int ProductID { get; set; }
        
        [Required]
        public DateTime DateAdded { get; set; } = DateTime.Now;
        
        // Navigation properties
        [ForeignKey("FavoriteID")]
        public virtual Favorite Favorite { get; set; }
        
        [ForeignKey("ProductID")]
        public virtual Product Product { get; set; }
    }
}