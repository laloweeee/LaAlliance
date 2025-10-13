using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ASI.Basecode.Data.Configuration
{
    public class OrderProcessedConfiguration : IEntityTypeConfiguration<OrderProcessed>
    {
        public void Configure(EntityTypeBuilder<OrderProcessed> builder)
        {
            // Primary Key
            builder.HasKey(op => op.OrderProcessID);

            // Properties
            builder.Property(op => op.ElapsedTime)
                .IsRequired();

            builder.Property(op => op.ProcessedAt)
                .IsRequired();

            // Relationships are configured in OrderConfiguration and RestaurantStaffConfiguration

            // Table Name
            builder.ToTable("OrderProcessed");
        }
    }
}
