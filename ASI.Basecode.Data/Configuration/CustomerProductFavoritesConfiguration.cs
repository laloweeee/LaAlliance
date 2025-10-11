using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ASI.Basecode.Data.Configuration
{
    public class CustomerProductFavoritesConfiguration : IEntityTypeConfiguration<CustomerProductFavorites>
    {
        public void Configure(EntityTypeBuilder<CustomerProductFavorites> builder)
        {
            // Primary Key
            builder.HasKey(cpf => cpf.CustomerProductFavoriteID);

            // Properties
            builder.Property(cpf => cpf.DateAdded)
                .IsRequired();

            // Relationships are configured in UserConfiguration and ProductConfiguration

            // Table Name
            builder.ToTable("CustomerProductFavorites");
        }
    }
}
