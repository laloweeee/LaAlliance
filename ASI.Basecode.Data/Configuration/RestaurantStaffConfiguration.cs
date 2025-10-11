using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Data.Configuration
{
    public class RestaurantStaffConfiguration : IEntityTypeConfiguration<RestaurantStaff>
    {
        public void Configure(EntityTypeBuilder<RestaurantStaff> builder)
        {
            // Primary Key
            builder.HasKey(rs => rs.StaffID);

            // Properties
            builder.Property(rs => rs.Role)
                .IsRequired()
                .HasConversion<string>()
                .HasDefaultValue(StaffRole.Staff);

            builder.Property(rs => rs.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasDefaultValue(AccountStatus.Active);

            // Relationships
            builder.HasMany(rs => rs.OrderProcessed)
                .WithOne(op => op.HandledBy)
                .HasForeignKey(op => op.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            // Table Name
            builder.ToTable("RestaurantStaff");
        }
    }
}
