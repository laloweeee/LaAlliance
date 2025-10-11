using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ASI.Basecode.Data.Configuration
{
    public class PromotionCodesConfiguration : IEntityTypeConfiguration<PromotionCodes>
    {
        public void Configure(EntityTypeBuilder<PromotionCodes> builder)
        {
            // Primary Key
            builder.HasKey(pc => pc.PromotionCodeID);

            // Properties
            builder.Property(pc => pc.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(pc => pc.UsageLimit)
                .IsRequired();

            builder.Property(pc => pc.UsedCount)
                .IsRequired();

            builder.Property(pc => pc.ExpirationDate)
                .IsRequired();

            // Relationships are configured in RestaurantPromotionsConfiguration

            // Table Name
            builder.ToTable("PromotionCodes");
        }
    }
}
