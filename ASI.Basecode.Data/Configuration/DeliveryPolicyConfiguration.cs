using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ASI.Basecode.Data.Configuration
{
    public class DeliveryPolicyConfiguration : IEntityTypeConfiguration<DeliveryPolicy>
    {
        public void Configure(EntityTypeBuilder<DeliveryPolicy> builder)
        {
            // Primary Key
            builder.HasKey(dp => dp.PolicyID);

            // Properties
            builder.Property(dp => dp.MaxDeliveryDistance)
                .IsRequired();

            builder.Property(dp => dp.BaseDeliveryFee)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(dp => dp.PerKmFee)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(dp => dp.MinimumOrderAmount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(dp => dp.EffectiveDate)
                .IsRequired();

            builder.Property(dp => dp.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Table Name
            builder.ToTable("DeliveryPolicies");
        }
    }
}
