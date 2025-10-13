using Microsoft.EntityFrameworkCore;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Data.Configuration;
using System;

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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql("Server=laalliance-giepoint.c.aivencloud.com;Port=21352;Database=laalliance;User=avnadmin;Password=AVNS_0AHFUgbnghTBf4VkfTZ;SslMode=Required", new MySqlServerVersion(new Version(8, 0, 21)));
            }
        }

        // User related
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserProfile> UserProfiles { get; set; }
        public virtual DbSet<UserAddress> UserAddresses { get; set; }

        // Restaurant related
        public virtual DbSet<Restaurant> Restaurants { get; set; }
        public virtual DbSet<RestaurantAddress> RestaurantAddresses { get; set; }
        public virtual DbSet<RestaurantStaff> RestaurantStaff { get; set; }

        // Product related
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductCategory> ProductCategories { get; set; }
        public virtual DbSet<ProductOptionGroup> ProductOptionGroups { get; set; }
        public virtual DbSet<ProductOptionItems> ProductOptionItems { get; set; }

        // Cart related
        public virtual DbSet<Cart> Carts { get; set; }
        public virtual DbSet<CartItem> CartItems { get; set; }
        public virtual DbSet<CartItemOption> CartItemOptions { get; set; }

        // Order related
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderItems> OrderItems { get; set; }
        public virtual DbSet<OrderItemOption> OrderItemOptions { get; set; }
        public virtual DbSet<OrderProcessed> OrderProcessed { get; set; }

        // Promotion related
        public virtual DbSet<RestaurantPromotions> RestaurantPromotions { get; set; }
        public virtual DbSet<PromotionCodes> PromotionCodes { get; set; }
        public virtual DbSet<PromotionProducts> PromotionProducts { get; set; }

        // Other
        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<DeliveryPolicy> DeliveryPolicies { get; set; }
        public virtual DbSet<PaymentLog> PaymentLogs { get; set; }
        public virtual DbSet<CustomerProductFavorites> CustomerProductFavorites { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new UserProfileConfiguration());
            modelBuilder.ApplyConfiguration(new UserAddressConfiguration());
            
            modelBuilder.ApplyConfiguration(new RestaurantConfiguration());
            modelBuilder.ApplyConfiguration(new RestaurantAddressConfiguration());
            modelBuilder.ApplyConfiguration(new RestaurantStaffConfiguration());
            
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.ApplyConfiguration(new ProductCategoryConfiguration());
            modelBuilder.ApplyConfiguration(new ProductOptionGroupConfiguration());
            modelBuilder.ApplyConfiguration(new ProductOptionItemsConfiguration());
            
            modelBuilder.ApplyConfiguration(new CartConfiguration());
            modelBuilder.ApplyConfiguration(new CartItemConfiguration());
            modelBuilder.ApplyConfiguration(new CartItemOptionConfiguration());
            
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
            modelBuilder.ApplyConfiguration(new OrderItemsConfiguration());
            modelBuilder.ApplyConfiguration(new OrderItemOptionConfiguration());
            modelBuilder.ApplyConfiguration(new OrderProcessedConfiguration());
            
            modelBuilder.ApplyConfiguration(new RestaurantPromotionsConfiguration());
            modelBuilder.ApplyConfiguration(new PromotionCodesConfiguration());
            modelBuilder.ApplyConfiguration(new PromotionProductsConfiguration());
            
            modelBuilder.ApplyConfiguration(new AddressConfiguration());
            modelBuilder.ApplyConfiguration(new DeliveryPolicyConfiguration());
            modelBuilder.ApplyConfiguration(new PaymentLogConfiguration());
            modelBuilder.ApplyConfiguration(new CustomerProductFavoritesConfiguration());

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
