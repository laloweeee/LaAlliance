using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ASI.Basecode.Data.Configuration
{
    public class OrderItemOptionConfiguration : IEntityTypeConfiguration<OrderItemOption>
    {
        public void Configure(EntityTypeBuilder<OrderItemOption> builder)
        {
            // Primary Key
            builder.HasKey(oio => oio.OrderItemOptionID);

            // Properties
            builder.Property(oio => oio.OrderItemID)
                .IsRequired();

            builder.Property(oio => oio.ProductOptionGroupID)
                .IsRequired();

            builder.Property(oio => oio.ProductOptionItemID)
                .IsRequired();

            // Relationships are configured in OrderItemsConfiguration, ProductOptionGroupConfiguration, and ProductOptionItemsConfiguration

            // Table Name
            builder.ToTable("OrderItemOptions");
        }
    }
}
