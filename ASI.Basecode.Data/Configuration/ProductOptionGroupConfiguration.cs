using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ASI.Basecode.Data.Configuration
{
    public class ProductOptionGroupConfiguration : IEntityTypeConfiguration<ProductOptionGroup>
    {
        public void Configure(EntityTypeBuilder<ProductOptionGroup> builder)
        {
            // Primary Key
            builder.HasKey(pog => pog.ProductOptionGroupID);

            // Properties
            builder.Property(pog => pog.OptionGroupName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(pog => pog.IsRequired)
                .IsRequired();

            builder.Property(pog => pog.NumberOfChoice)
                .IsRequired();

            // Relationships
            builder.HasOne(pog => pog.CartItemOption)
                .WithOne(cio => cio.ProductOptionGroup)
                .HasForeignKey<CartItemOption>(cio => cio.ProductOptionGroupID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(pog => pog.ProductOptionItems)
                .WithOne(poi => poi.ProductOptionGroup)
                .HasForeignKey(poi => poi.ProductOptionGroupID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(pog => pog.OrderItemOption)
                .WithOne(oio => oio.ProductOptionGroup)
                .HasForeignKey(oio => oio.ProductOptionGroupID)
                .OnDelete(DeleteBehavior.Restrict);

            // Table Name
            builder.ToTable("ProductOptionGroups");
        }
    }
}
