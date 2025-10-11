using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ASI.Basecode.Data.Configuration
{
    public class RestaurantAddressConfiguration : IEntityTypeConfiguration<RestaurantAddress>
    {
        public void Configure(EntityTypeBuilder<RestaurantAddress> builder)
        {
            // Primary Key
            builder.HasKey(ra => ra.RestaurantAddressID);

            // Properties
            builder.Property(ra => ra.RestaurantID)
                .IsRequired();

            builder.Property(ra => ra.AddressID)
                .IsRequired();

            // Relationships
            builder.HasOne(ra => ra.Address)
                .WithOne(a => a.RestaurantAddress)
                .HasForeignKey<RestaurantAddress>(ra => ra.AddressID)
                .OnDelete(DeleteBehavior.Restrict);

            // Table Name
            builder.ToTable("RestaurantAddresses");
        }
    }
}
