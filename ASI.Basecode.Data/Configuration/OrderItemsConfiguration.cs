using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ASI.Basecode.Data.Configuration
{
    public class OrderItemsConfiguration : IEntityTypeConfiguration<OrderItems>
    {
        public void Configure(EntityTypeBuilder<OrderItems> builder)
        {
            // Primary Key
            builder.HasKey(oi => oi.OderItemID);

            // Properties
            builder.Property(oi => oi.Quantity)
                .IsRequired();

            builder.Property(oi => oi.UnitPrice)
                .IsRequired()
                .HasPrecision(18, 2);

            // Relationships
            builder.HasMany(oi => oi.OrderItemOption)
                .WithOne(oio => oio.OrderItems)
                .HasForeignKey(oio => oio.OrderItemID)
                .OnDelete(DeleteBehavior.Cascade);

            // Table Name
            builder.ToTable("OrderItems");
        }
    }
}
