using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ASI.Basecode.Data.Configuration
{
    public class PromotionProductsConfiguration : IEntityTypeConfiguration<PromotionProducts>
    {
        public void Configure(EntityTypeBuilder<PromotionProducts> builder)
        {
            // Composite Primary Key
            builder.HasKey(pp => new { pp.PromotionID, pp.ProductID });

            // Relationships are configured in RestaurantPromotionsConfiguration and ProductConfiguration

            // Table Name
            builder.ToTable("PromotionProducts");
        }
    }
}
