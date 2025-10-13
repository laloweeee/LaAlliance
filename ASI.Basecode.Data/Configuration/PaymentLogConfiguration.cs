using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Data.Configuration
{
    public class PaymentLogConfiguration : IEntityTypeConfiguration<PaymentLog>
    {
        public void Configure(EntityTypeBuilder<PaymentLog> builder)
        {
            // Primary Key
            builder.HasKey(pl => pl.PaymentLogID);

            // Properties
            builder.Property(pl => pl.PaymentMethod)
                .IsRequired()
                .HasConversion<string>()
                .HasDefaultValue(PaymentMethod.CreditCard);

            builder.Property(pl => pl.PaymentStatus)
                .IsRequired()
                .HasConversion<string>()
                .HasDefaultValue(PaymentStatus.Pending);

            builder.Property(pl => pl.TransactionReference)
                .HasMaxLength(255);

            builder.Property(pl => pl.PaymentAmount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(pl => pl.PaymentDate)
                .IsRequired();

            builder.Property(pl => pl.Remarks)
                .HasMaxLength(1000);

            builder.Property(pl => pl.CreatedAt)
                .IsRequired();

            // Relationships
            builder.HasOne(pl => pl.User)
                .WithMany()
                .HasForeignKey(pl => pl.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            // Table Name
            builder.ToTable("PaymentLogs");
        }
    }
}
