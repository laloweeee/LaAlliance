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
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> entity)
        {
            entity.ToTable("Categories");

            entity.HasKey(e => e.CategoryID);

            entity.Property(e => e.CategoryID)
                  .HasColumnName("CategoryID")
                  .ValueGeneratedOnAdd();

            entity.Property(e => e.Name)
                  .HasColumnName("Name")
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(e => e.CreatedBy)
                  .HasColumnName("CreatedBy")
                  .IsRequired();

            entity.Property(e => e.IsActive)
                  .HasColumnName("IsActive")
                  .IsRequired()
                  .HasDefaultValue(true);

            entity.Property(e => e.CreatedTime)
                  .HasColumnName("CreatedTime")
                  .HasColumnType("datetime")
                  .IsRequired()
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.UpdatedBy)
                  .HasColumnName("UpdatedBy")
                  .IsRequired();

            entity.Property(e => e.UpdatedTime)
                  .HasColumnName("UpdatedTime")
                  .HasColumnType("datetime")
                  .IsRequired()
                  .HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

            entity.HasOne(e => e.CreatedByUser)
                  .WithMany()
                  .HasForeignKey(e => e.CreatedBy);

            entity.HasOne(e => e.UpdatedByUser)
                  .WithMany()
                  .HasForeignKey(e => e.UpdatedBy);
        }
    }
}
