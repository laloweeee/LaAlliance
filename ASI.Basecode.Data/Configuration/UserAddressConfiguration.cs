using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ASI.Basecode.Data.Configuration
{
    public class UserAddressConfiguration : IEntityTypeConfiguration<UserAddress>
    {
        public void Configure(EntityTypeBuilder<UserAddress> builder)
        {
            // Primary Key
            builder.HasKey(ua => ua.UserAddressID);

            // Properties
            builder.Property(ua => ua.IsDefault)
                .HasDefaultValue(true);

            builder.Property(ua => ua.AddressType)
                .HasMaxLength(50);

            builder.Property(ua => ua.AddressNote)
                .HasMaxLength(500);

            // Relationships
            builder.HasOne(ua => ua.Address)
                .WithOne(a => a.UserAddress)
                .HasForeignKey<UserAddress>(ua => ua.AddressID)
                .OnDelete(DeleteBehavior.Restrict);

            // Table Name
            builder.ToTable("UserAddresses");
        }
    }
}
