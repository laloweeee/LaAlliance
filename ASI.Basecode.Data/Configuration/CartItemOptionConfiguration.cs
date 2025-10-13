using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ASI.Basecode.Data.Configuration
{
    public class CartItemOptionConfiguration : IEntityTypeConfiguration<CartItemOption>
    {
        public void Configure(EntityTypeBuilder<CartItemOption> builder)
        {
            // Primary Key
            builder.HasKey(cio => cio.CartItemOptionID);

            // Properties
            builder.Property(cio => cio.CartItemID)
                .IsRequired();

            builder.Property(cio => cio.ProductOptionGroupID)
                .IsRequired();

            builder.Property(cio => cio.ProductOptionItemID)
                .IsRequired();

            // Table Name
            builder.ToTable("CartItemOptions");
        }
    }
}
