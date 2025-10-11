using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ASI.Basecode.Data.Configuration
{
    public class RestaurantConfiguration : IEntityTypeConfiguration<Restaurant>
    {
        public void Configure(EntityTypeBuilder<Restaurant> builder)
        {
            // Primary Key
            builder.HasKey(r => r.RestaurantID);

            // Properties
            builder.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(r => r.Description)
                .HasMaxLength(1000);

            builder.Property(r => r.ContactNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(r => r.Email)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(r => r.OpeningTime)
                .IsRequired();

            builder.Property(r => r.ClosingTime)
                .IsRequired();

            // Relationships
            builder.HasOne(r => r.RestaurantAddress)
                .WithOne(ra => ra.Restaurant)
                .HasForeignKey<RestaurantAddress>(ra => ra.RestaurantID)
                .OnDelete(DeleteBehavior.Cascade);

            // Table Name
            builder.ToTable("Restaurants");
        }
    }
}
