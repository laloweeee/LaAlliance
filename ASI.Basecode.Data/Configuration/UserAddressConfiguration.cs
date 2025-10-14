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
            // Many UserAddresses can reference one Address
            builder.HasOne(ua => ua.Address)
                .WithMany(a => a.UserAddresses)
                .HasForeignKey(ua => ua.AddressID)
                .OnDelete(DeleteBehavior.Restrict);

            // Many UserAddresses can belong to one User
            builder.HasOne(ua => ua.User)
                .WithMany(u => u.UserAddress)
                .HasForeignKey(ua => ua.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            // Table Name
            builder.ToTable("UserAddresses");
        }
    }
}
