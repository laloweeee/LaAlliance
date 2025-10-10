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
    public class CustomizationOptionConfiguration : IEntityTypeConfiguration<ProductOptionItems>
    {
        public void Configure(EntityTypeBuilder<ProductOptionItems> entity)
        {
            entity.ToTable("CustomizationOptions");

            entity.HasKey(e => e.CustomizationOptionID);

            entity.Property(e => e.CustomizationOptionID)
                    .HasColumnName("CustomizationOptionID")
                    .ValueGeneratedOnAdd();

            entity.Property(e => e.CustomizationGroupID)
                    .HasColumnName("CustomizationGroupID")
                    .IsRequired();

            entity.Property(e => e.OptionName)
                    .HasColumnName("OptionName")
                    .IsRequired()
                    .HasMaxLength(100);

            entity.Property(e => e.AdditionalPrice)
                    .HasColumnName("AdditionalPrice")
                    .IsRequired()
                    .HasColumnType("decimal(10, 2)")
                    .HasDefaultValue(0.00m);

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

            entity.HasOne(e => e.CustomizationGroup)
                    .WithMany(cg => cg.CustomizationOptions)
                    .HasForeignKey(e => e.CustomizationGroupID);
        }
    }
}
