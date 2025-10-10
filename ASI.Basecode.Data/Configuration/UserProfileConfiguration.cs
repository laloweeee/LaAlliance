using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ASI.Basecode.Data.Configuration
{
      public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
      {
            public void Configure(EntityTypeBuilder<UserProfile> entity)
            {
                  entity.ToTable("UserProfiles");

                  entity.HasKey(e => e.UserProfileId);

                  entity.Property(e => e.UserProfileId)
                        .HasColumnName("UserProfileId");

                  entity.Property(e => e.ContactNumber)
                        .HasColumnName("ContactNumber")
                        .HasMaxLength(15);

                  entity.Property(e => e.UserPhoto)
                        .HasColumnName("UserPhoto")
                        .HasMaxLength(255);

                  entity.Property(e => e.CreatedTime)
                        .HasColumnName("CreatedTime")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                  entity.Property(e => e.UpdatedTime)
                        .HasColumnName("UpdatedTime")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

                  // Relationships
                  entity.HasOne(e => e.User)
                        .WithOne(u => u.UserProfile)
                        .HasForeignKey<UserProfile>(e => e.UserProfileId)
                        .OnDelete(DeleteBehavior.Cascade);
            }
      }
}