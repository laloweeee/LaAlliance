using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ASI.Basecode.Data.Configuration
{
    public class UserAddressConfiguration : IEntityTypeConfiguration<UserAddress>
    {
        public void Configure(EntityTypeBuilder<UserAddress> entity)
        {
            entity.ToTable("UserAddresses");

            entity.HasKey(e => e.UserAddressId);

            entity.Property(e => e.UserAddressId)
                .HasColumnName("UserAddressId")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.UserId)
                .HasColumnName("UserId")
                .IsRequired();

            entity.Property(e => e.AddressId)
                .HasColumnName("AddressId")
                .IsRequired();

            entity.Property(e => e.IsDefault)
                .HasColumnName("IsDefault")
                .IsRequired();

            entity.Property(e => e.AddressLabel)
                .HasColumnName("AddressLabel")
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.AddressNote)
                .HasColumnName("AddressNote")
                .HasMaxLength(250);

            entity.HasOne(e => e.User)
                .WithMany(u => u.UserAddresses)
                .HasForeignKey(e => e.UserId);

            entity.HasOne(e => e.Address)
                .WithOne()
                .HasForeignKey<UserAddress>(e => e.AddressId);
        }
    }
}