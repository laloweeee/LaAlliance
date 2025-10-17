using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Data.Configuration
{
    public class RestaurantPromotionsConfiguration : IEntityTypeConfiguration<RestaurantPromotions>
    {
        public void Configure(EntityTypeBuilder<RestaurantPromotions> builder)
        {
            // Primary Key
            builder.HasKey(rp => rp.PromotionID);
            
            // Configure PromotionID as auto-generated identity
            builder.Property(rp => rp.PromotionID)
                .ValueGeneratedOnAdd();

            // Properties
            builder.Property(rp => rp.PromotionName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(rp => rp.PromotionBanner)
                .HasMaxLength(500);

            builder.Property(rp => rp.PromotionDescription)
                .HasMaxLength(1000);

            builder.Property(rp => rp.DiscountType)
                .IsRequired()
                .HasConversion(
                    v => v.ToString(),
                    v => ConvertStringToDiscountType(v))
                .HasDefaultValue(DiscountType.Percentage);

            builder.Property(rp => rp.DiscountValue)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(rp => rp.MinimumOrderAmount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(rp => rp.StartDate)
                .IsRequired();

            builder.Property(rp => rp.EndDate)
                .IsRequired();

            builder.Property(rp => rp.IsActive)
                .HasDefaultValue(true);

            // Relationships
            builder.HasOne(rp => rp.PromotionCodes)
                .WithOne(pc => pc.RestaurantPromotions)
                .HasForeignKey<PromotionCodes>(pc => pc.PromotionID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(rp => rp.PromotionProducts)
                .WithMany(pp => pp.RestaurantPromotions);

            builder.HasMany(rp => rp.Order)
                .WithOne(o => o.RestaurantPromotion)
                .HasForeignKey(o => o.PromotionID)
                .OnDelete(DeleteBehavior.Restrict);

            // Table Name
            builder.ToTable("RestaurantPromotions");
        }

        private static DiscountType ConvertStringToDiscountType(string value)
        {
            return value switch
            {
                "Percentage" => DiscountType.Percentage,
                "Fixed Amount" => DiscountType.FixedAmount,
                "FixedAmount" => DiscountType.FixedAmount,
                _ => DiscountType.Percentage
            };
        }
    }
}
