using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ASI.Basecode.Data.Configuration
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            // Primary Key
            builder.HasKey(p => p.ProductID);

            // Properties
            builder.Property(p => p.ProductName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.ProductImage)
                .HasMaxLength(500);

            builder.Property(p => p.ProductDescription)
                .HasMaxLength(1000);

            builder.Property(p => p.ProductPrice)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(p => p.IsActive)
                .IsRequired();

            // Relationships
            builder.HasOne(p => p.ProductCategory)
                .WithMany(pc => pc.Products)
                .HasForeignKey(p => p.CategoryID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.OrderItems)
                .WithOne(oi => oi.Product)
                .HasForeignKey(oi => oi.ProductID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.PromotionProducts)
                .WithOne(pp => pp.Product)
                .HasForeignKey(pp => pp.ProductID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.ProductOptionGroup)
                .WithOne(pog => pog.Product)
                .HasForeignKey(pog => pog.ProductID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.CustomerProductFavorites)
                .WithOne(cpf => cpf.Product)
                .HasForeignKey(cpf => cpf.ProductID)
                .OnDelete(DeleteBehavior.Cascade);

            // Table Name
            builder.ToTable("Products");
        }
    }
}
