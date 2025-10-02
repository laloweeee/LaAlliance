using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ASI.Basecode.Data.Configuration
{
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> entity)
        {
            entity.ToTable("Addresses");

            entity.HasKey(e => e.AddressId);

            entity.Property(e => e.AddressId)
                  .HasColumnName("AddressID")
                  .ValueGeneratedOnAdd();

            entity.Property(e => e.StreetAddress)
                  .HasColumnName("StreetAddress")
                  .IsRequired()
                  .HasMaxLength(200);

            entity.Property(e => e.Barangay)
                  .HasColumnName("Barangay")
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(e => e.City)
                  .HasColumnName("City")
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(e => e.Province)
                  .HasColumnName("Province")
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(e => e.ZipCode)
                  .HasColumnName("ZipCode")
                  .IsRequired();

            entity.Property(e => e.Country)
                  .HasColumnName("Country")
                  .IsRequired()
                  .HasMaxLength(100)
                  .HasDefaultValue("Philippines");
        }
    }
}