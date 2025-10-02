using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ASI.Basecode.Data.Configuration
{
    public class ServiceDataConfiguration : IEntityTypeConfiguration<ServiceData>
    {
        public void Configure(EntityTypeBuilder<ServiceData> entity)
        {
            entity.ToTable("ServiceData");

            entity.HasKey(e => e.ServiceDataId);

            entity.Property(e => e.ServiceDataId)
                  .HasColumnName("ServiceDataId")
                  .ValueGeneratedOnAdd();

            entity.Property(e => e.RestaurantId)
                  .HasColumnName("RestaurantId")
                  .IsRequired();

            entity.Property(e => e.MaxDeliveryDistance)
                  .HasColumnName("MaxDeliveryDistance")
                  .IsRequired();

            entity.Property(e => e.BaseDeliveryFee)
                  .HasColumnName("BaseDeliveryFee")
                  .HasColumnType("decimal(10, 2)")
                  .IsRequired();

            entity.Property(e => e.PerKmFee)
                  .HasColumnName("PerKmFee")
                  .HasColumnType("decimal(10, 2)")
                  .IsRequired();

            entity.Property(e => e.MinimumOrderAmount)
                  .HasColumnName("MinimumOrderAmount")
                  .HasColumnType("decimal(10, 2)")
                  .IsRequired();

            // Relationships
            entity.HasOne(e => e.RestaurantProfile)
                  .WithOne(rp => rp.ServiceData)
                  .HasForeignKey<ServiceData>(e => e.RestaurantId)
                  .OnDelete(DeleteBehavior.Cascade);
        }
    }
}