using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Data.Configuration
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            // Primary Key
            builder.HasKey(o => o.OrderID);

            // Properties
            builder.Property(o => o.OrderDate)
                .IsRequired();

            builder.Property(o => o.SubTotal)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(o => o.DiscountAmount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(o => o.TotalAmount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(o => o.OrderType)
                .IsRequired()
                .HasConversion<string>()
                .HasDefaultValue(OrderType.Delivery);

            builder.Property(o => o.OrderStatus)
                .IsRequired()
                .HasConversion<string>()
                .HasDefaultValue(OrderStatus.Pending);

            builder.Property(o => o.PaymentMethod)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            // Relationships
            builder.HasOne(o => o.Address)
                .WithOne(a => a.Order)
                .HasForeignKey<Order>(o => o.OrderAddressID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(o => o.OrderProcessed)
                .WithOne(op => op.Order)
                .HasForeignKey<OrderProcessed>(op => op.OrderID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OderID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(o => o.PaymentLogs)
                .WithOne(pl => pl.Order)
                .HasForeignKey(pl => pl.OrderID)
                .OnDelete(DeleteBehavior.Cascade);

            // Table Name
            builder.ToTable("Orders");
        }
    }
}
