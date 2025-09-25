using Microsoft.EntityFrameworkCore;
using ASI.Basecode.Data.Models;

namespace ASI.Basecode.Data
{
    public partial class AsiBasecodeDBContext : DbContext
    {
        public AsiBasecodeDBContext()
        {
        }

        public AsiBasecodeDBContext(DbContextOptions<AsiBasecodeDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<User> Users { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");

                entity.HasKey(e => e.UserId);

                entity.Property(e => e.UserId)
                    .HasColumnName("UserId")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Email)
                    .HasColumnName("Email")
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Password)
                    .HasColumnName("Password")
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Role)
                    .HasColumnName("Role")
                    .IsRequired()
                    .HasMaxLength(50);

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

                entity.Property(e => e.UpdatedTime)
                    .HasColumnName("UpdatedTime")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CreatedTime)
                    .HasColumnName("CreatedTime")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
