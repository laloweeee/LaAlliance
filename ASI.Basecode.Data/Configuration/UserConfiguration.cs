using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> entity)
        {
            entity.ToTable("Users");

            entity.HasKey(e => e.UserID);

            entity.Property(e => e.UserID)
                  .HasColumnName("UserID")
                  .ValueGeneratedOnAdd();

            entity.Property(e => e.Email)
                  .HasColumnName("Email")
                  .IsRequired()
                  .HasMaxLength(255);

            entity.Property(e => e.Password)
                  .HasColumnName("Password")
                  .IsRequired()
                  .HasMaxLength(255);

            entity.Property(e => e.FirstName)
                  .HasColumnName("FirstName")
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(e => e.LastName)
                  .HasColumnName("LastName")
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(e => e.Role)
                  .HasColumnName("Role")
                  .IsRequired()
                  .HasMaxLength(50)
                  .HasDefaultValue("Customer");

            entity.Property(e => e.IsEmailVerified)
                  .HasColumnName("IsEmailVerified")
                  .HasDefaultValue(false);

            entity.Property(e => e.EmailHashToken)
                  .HasColumnName("EmailHashToken")
                  .HasMaxLength(512);

            entity.Property(e => e.EmailTokenExpiry)
                  .HasColumnName("EmailTokenExpiry")
                  .HasColumnType("datetime");

            entity.Property(e => e.ResetPasswordHashToken)
                  .HasColumnName("ResetPasswordHashToken")
                  .HasMaxLength(512);

            entity.Property(e => e.ResetTokenExpiry)
                  .HasColumnName("ResetTokenExpiry")
                  .HasColumnType("datetime");

            entity.Property(e => e.CreatedTime)
                  .HasColumnName("CreatedTime")
                  .HasColumnType("datetime")
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.UpdatedTime)
                  .HasColumnName("UpdatedTime")
                  .HasColumnType("datetime")
                  .HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

            entity.HasOne(e => e.UserProfile)
                  .WithOne(up => up.User)
                  .HasForeignKey<UserProfile>(up => up.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
