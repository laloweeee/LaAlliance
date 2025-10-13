using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ASI.Basecode.Data.Configuration
{
    public class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
    {
        public void Configure(EntityTypeBuilder<ProductCategory> builder)
        {
            // Primary Key
            builder.HasKey(pc => pc.CategoryID);

            // Properties
            builder.Property(pc => pc.CategoryName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(pc => pc.IsActive)
                .IsRequired();

            // Relationships are configured in ProductConfiguration

            // Table Name
            builder.ToTable("ProductCategories");
        }
    }
}
