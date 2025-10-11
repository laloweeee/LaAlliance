using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ASI.Basecode.Data.Configuration
{
    public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
    {
        public void Configure(EntityTypeBuilder<UserProfile> builder)
        {
            // Primary Key
            builder.HasKey(up => up.ProfileID);

            // Properties
            builder.Property(up => up.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(up => up.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(up => up.ContactNumber)
                .IsRequired();

            // Relationships are configured in UserConfiguration

            // Table Name
            builder.ToTable("UserProfiles");
        }
    }
}
