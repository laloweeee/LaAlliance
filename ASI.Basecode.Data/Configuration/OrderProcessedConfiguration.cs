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

            // SIMPLIFIED relationship - no back-reference
            builder.HasOne(op => op.HandledBy)
                .WithMany()  // No navigation property on the other side
                .HasForeignKey(op => op.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            // Table Name
            builder.ToTable("OrderProcessed");
        }
    }
}