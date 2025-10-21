using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASI.Basecode.Data.Models
{
    public class Favorite
    {
        [Key]
        public int FavoriteID { get; set; }
        
        [Required]
        public int UserID { get; set; }
        
        [Required]
        public DateTime DateCreated { get; set; } = DateTime.Now;
        
        // Navigation properties
        [ForeignKey("UserID")]
        public virtual User User { get; set; }
        
        public virtual ICollection<FavoriteItem> FavoriteItems { get; set; } = new List<FavoriteItem>();
    }
}