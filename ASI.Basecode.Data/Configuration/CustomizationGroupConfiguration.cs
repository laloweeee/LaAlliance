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
    public class CustomizationGroupConfiguration : IEntityTypeConfiguration<CustomizationGroup>
    {
        public void Configure(EntityTypeBuilder<CustomizationGroup> entity)
        {
            entity.ToTable("CustomizationGroups");

            entity.HasKey(e => e.CustomizationGroupID);

            entity.Property(e => e.CustomizationGroupID)
                    .HasColumnName("CustomizationGroupID")
                    .ValueGeneratedOnAdd();

            entity.Property(e => e.ProductID)
                    .HasColumnName("ProductID")
                    .IsRequired();

            entity.Property(e => e.CustomizationName)
                    .HasColumnName("CustomizationName")
                    .IsRequired()
                    .HasMaxLength(100);

            entity.Property(e => e.IsRequired)
                    .HasColumnName("IsRequired")
                    .IsRequired();

            entity.Property(e => e.IsSingleChoice)
                    .HasColumnName("IsSingleChoice")
                    .IsRequired();

            entity.Property(e => e.CreatedTime)
                    .HasColumnName("CreatedTime")
                    .HasColumnType("datetime")
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.UpdatedTime)
                    .HasColumnName("UpdatedTime")
                    .HasColumnType("datetime")
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

            entity.HasOne(e => e.Product)
                    .WithMany(p => p.CustomizationGroups)
                    .HasForeignKey(e => e.ProductID);
        }
    }
}
