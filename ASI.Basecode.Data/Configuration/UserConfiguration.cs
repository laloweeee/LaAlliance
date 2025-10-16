using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Data.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Primary Key
            builder.HasKey(u => u.UserID);

            // Properties
            builder.Property(u => u.Email)
                .IsRequired()
                .HasColumnType("varchar(255)")
                .HasMaxLength(255);

            builder.Property(u => u.Password)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(u => u.UserType)
                .IsRequired()
                .HasConversion<string>()
                .HasDefaultValue(UserType.Customer);

            builder.Property(u => u.IsEmailVerified)
                .HasDefaultValue(false);

            builder.Property(u => u.AccountStatus)
                .IsRequired()
                .HasConversion<string>()
                .HasDefaultValue(AccountStatus.Active);

            // Relationships
            builder.HasOne(u => u.UserProfile)
                .WithOne(up => up.User)
                .HasForeignKey<UserProfile>(up => up.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.UserAddress)
                .WithOne(ua => ua.User)
                .HasForeignKey(ua => ua.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(u => u.RestaurantStaff)
                .WithOne(rs => rs.User)
                .HasForeignKey<RestaurantStaff>(rs => rs.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(u => u.Cart)
                .WithOne(c => c.User)
                .HasForeignKey<Cart>(c => c.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.CustomerProductFavorites)
                .WithOne(cpf => cpf.User)
                .HasForeignKey(cpf => cpf.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.Order)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.EmailVerificationTokens)
                .WithOne(evt => evt.User)
                .HasForeignKey(evt => evt.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.PasswordResetTokens)
                .WithOne(prt => prt.User)
                .HasForeignKey(prt => prt.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            // Table Name
            builder.ToTable("Users");
        }
    }
}
