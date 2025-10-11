using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ASI.Basecode.Data.Configuration
{
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            // Primary Key
            builder.HasKey(a => a.AddressID);

            // Properties
            builder.Property(a => a.Longitude)
                .IsRequired();

            builder.Property(a => a.Latitude)
                .IsRequired();

            builder.Property(a => a.Street)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(a => a.Barangay)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.City)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.Province)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.ZipCode)
                .IsRequired();

            builder.Property(a => a.Country)
                .HasMaxLength(100)
                .HasDefaultValue("Philippines");

            // Relationships are configured in UserAddressConfiguration, RestaurantAddressConfiguration, and OrderConfiguration

            // Table Name
            builder.ToTable("Addresses");
        }
    }
}
