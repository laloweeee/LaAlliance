using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ASI.Basecode.Data.Configuration
{
        public class RestaurantProfileConfiguration : IEntityTypeConfiguration<RestaurantProfile>
        {
                public void Configure(EntityTypeBuilder<RestaurantProfile> entity)
                {
                        entity.ToTable("RestaurantProfiles");

                        entity.HasKey(e => e.RestaurantID);

                        entity.Property(e => e.RestaurantID)
                                .HasColumnName("RestaurantId")
                                .ValueGeneratedOnAdd();

                        entity.Property(e => e.Name)
                                .HasColumnName("Name")
                                .HasMaxLength(255)
                                .IsRequired();

                        entity.Property(e => e.Description)
                                .HasColumnName("Description")
                                .HasMaxLength(1000)
                                .IsRequired();

                        entity.Property(e => e.PhoneNumber)
                                .HasColumnName("PhoneNumber")
                                .HasMaxLength(15)
                                .IsRequired();

                        entity.Property(e => e.Email)
                                .HasColumnName("Email")
                                .HasMaxLength(255)
                                .IsRequired();

                        entity.Property(e => e.LogoUrl)
                                .HasColumnName("LogoUrl")
                                .HasMaxLength(500);

                        entity.Property(e => e.CoverImageUrl)
                                .HasColumnName("CoverImageUrl")
                                .HasMaxLength(500);

                        entity.Property(e => e.OpeningTime)
                                .HasColumnName("OpeningTime")
                                .HasColumnType("TIME")
                                .IsRequired();

                        entity.Property(e => e.ClosingTime)
                                .HasColumnName("ClosingTime")
                                .HasColumnType("TIME")
                                .IsRequired();

                        entity.Property(e => e.CreatedTime)
                                .HasColumnName("CreatedTime")
                                .HasColumnType("DATETIME")
                                .IsRequired()
                                .HasDefaultValueSql("CURRENT_TIMESTAMP");

                        entity.Property(e => e.UpdatedTime)
                                .HasColumnName("UpdatedTime")
                                .HasColumnType("DATETIME")
                                .IsRequired()
                                .HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

                        entity.Property(e => e.UpdatedBy)
                                .HasColumnName("UpdatedBy")
                                .IsRequired();

                        entity.HasOne(e => e.UpdatedByUser)
                                .WithMany()
                                .HasForeignKey(e => e.UpdatedBy)
                                .OnDelete(DeleteBehavior.Restrict);

                        entity.HasOne(e => e.Address)
                                .WithOne()
                                .HasForeignKey<RestaurantProfile>(e => e.AddressId);
                }
        }
}