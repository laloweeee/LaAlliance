using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ASI.Basecode.Data.Configuration
{
    public class ProductOptionItemsConfiguration : IEntityTypeConfiguration<ProductOptionItems>
    {
        public void Configure(EntityTypeBuilder<ProductOptionItems> builder)
        {
            // Primary Key
            builder.HasKey(poi => poi.ProductOptionItemsID);

            // Properties
            builder.Property(poi => poi.OptionName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(poi => poi.AdditionalPrice)
                .IsRequired()
                .HasPrecision(18, 2);

            // Relationships
            builder.HasOne(poi => poi.CartItemOption)
                .WithOne(cio => cio.ProductOptionItems)
                .HasForeignKey<CartItemOption>(cio => cio.ProductOptionItemID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(poi => poi.OrderItemOption)
                .WithOne(oio => oio.ProductOptionItems)
                .HasForeignKey(oio => oio.ProductOptionItemID)
                .OnDelete(DeleteBehavior.Restrict);

            // Table Name
            builder.ToTable("ProductOptionItems");
        }
    }
}
