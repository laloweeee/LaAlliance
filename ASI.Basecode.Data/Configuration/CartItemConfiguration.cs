using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ASI.Basecode.Data.Configuration
{
    public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            // Primary Key
            builder.HasKey(ci => ci.CartItemID);

            // Properties
            builder.Property(ci => ci.Quantity)
                .IsRequired();

            builder.Property(ci => ci.UnitPrice)
                .IsRequired()
                .HasPrecision(18, 2);

            // Relationships
            builder.HasOne(ci => ci.Product)
                .WithMany()
                .HasForeignKey(ci => ci.ProductID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(ci => ci.CartItemOption)
                .WithOne(cio => cio.CartItem)
                .HasForeignKey(cio => cio.CartItemID)
                .OnDelete(DeleteBehavior.Cascade);

            // Table Name
            builder.ToTable("CartItems");
        }
    }
}
