# Soft Delete Implementation Guide

## Overview
This project now implements a comprehensive soft delete strategy that allows for safe deletion of records while maintaining data integrity and audit trails.

## What Was Implemented

### 1. **BaseEntity & ISoftDeletable Interface**
All entities that support soft delete inherit from `BaseEntity` which implements `ISoftDeletable`:

```csharp
public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
    DateTime? DeletedDate { get; set; }
    string? DeletedBy { get; set; }
}
```

### 2. **Entities with Soft Delete Support**
The following entities now inherit from `BaseEntity` and support soft delete:

**Core Business Entities (Soft Delete)**:
- User
- UserProfile
- UserAddress
- Restaurant
- RestaurantAddress
- RestaurantStaff
- Product
- ProductCategory
- Address
- RestaurantPromotions
- PromotionCodes
- DeliveryPolicy
- CustomerProductFavorites

**Entities WITHOUT Soft Delete (Hard Delete or Never Delete)**:
- Cart/CartItem/CartItemOption (temporary data - hard delete)
- Order/OrderItems/OrderProcessed (never delete - audit requirement)
- VerificationTokens (security - hard delete)
- PaymentLog (archive separately)

### 3. **Global Query Filter**
A global query filter automatically excludes soft-deleted records from all queries:

```csharp
// Automatically applied to all queries
var activeUsers = context.Users.ToList(); // Only returns non-deleted users

// To include deleted records
var allUsers = context.Users.IgnoreQueryFilters().ToList();
```

### 4. **Enhanced BaseRepository Methods**

#### **SoftDelete**
Marks an entity as deleted without removing it from the database:

```csharp
protected void SoftDelete<TEntity>(TEntity entity, string deletedBy) 
    where TEntity : class, ISoftDeletable
{
    entity.IsDeleted = true;
    entity.DeletedDate = DateTime.UtcNow;
    entity.DeletedBy = deletedBy;
    Context.Update(entity);
}
```

#### **HardDelete**
Permanently removes an entity (use with caution):

```csharp
protected void HardDelete<TEntity>(TEntity entity) 
    where TEntity : class
{
    Context.Remove(entity);
}
```

#### **GetActive**
Retrieves only non-deleted entities:

```csharp
protected IQueryable<TEntity> GetActive<TEntity>() 
    where TEntity : class, ISoftDeletable
{
    return Context.Set<TEntity>().Where(x => !x.IsDeleted);
}
```

#### **GetAllIncludingDeleted**
Retrieves all entities including soft-deleted ones:

```csharp
protected IQueryable<TEntity> GetAllIncludingDeleted<TEntity>() 
    where TEntity : class
{
    return Context.Set<TEntity>().IgnoreQueryFilters();
}
```

#### **Restore**
Restores a soft-deleted entity:

```csharp
protected void Restore<TEntity>(TEntity entity) 
    where TEntity : class, ISoftDeletable
{
    entity.IsDeleted = false;
    entity.DeletedDate = null;
    entity.DeletedBy = null;
    Context.Update(entity);
}
```

## Usage Examples

### Example 1: Soft Delete a User

```csharp
public class UserRepository : BaseRepository, IUserRepository
{
    public void DeleteUser(int userId, string deletedBy)
    {
        var user = GetDbSet<User>().Find(userId);
        if (user != null)
        {
            SoftDelete(user, deletedBy);
            UnitOfWork.SaveChanges();
        }
    }
}
```

### Example 2: Hard Delete Cart Items (Temporary Data)

```csharp
public class CartRepository : BaseRepository, ICartRepository
{
    public void ClearCart(int cartId)
    {
        var cartItems = GetDbSet<CartItem>().Where(ci => ci.CartID == cartId);
        foreach (var item in cartItems)
        {
            HardDelete(item); // Permanently remove
        }
        UnitOfWork.SaveChanges();
    }
}
```

### Example 3: Get Active Products

```csharp
public class ProductRepository : BaseRepository, IProductRepository
{
    public IEnumerable<Product> GetActiveProducts()
    {
        // Global query filter automatically excludes soft-deleted products
        return GetDbSet<Product>().Where(p => p.IsActive).ToList();
        
        // Or explicitly use GetActive method
        return GetActive<Product>().Where(p => p.IsActive).ToList();
    }
}
```

### Example 4: Include Deleted Records for Admin View

```csharp
public class ProductRepository : BaseRepository, IProductRepository
{
    public IEnumerable<Product> GetAllProductsIncludingDeleted()
    {
        return GetAllIncludingDeleted<Product>().ToList();
    }
    
    public IEnumerable<Product> GetDeletedProducts()
    {
        return GetAllIncludingDeleted<Product>()
            .Where(p => p.IsDeleted)
            .ToList();
    }
}
```

### Example 5: Restore a Deleted Product

```csharp
public class ProductRepository : BaseRepository, IProductRepository
{
    public void RestoreProduct(int productId)
    {
        var product = GetAllIncludingDeleted<Product>()
            .FirstOrDefault(p => p.ProductID == productId);
            
        if (product != null && product.IsDeleted)
        {
            Restore(product);
            UnitOfWork.SaveChanges();
        }
    }
}
```

### Example 6: Service Layer Usage

```csharp
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    
    public void DeleteUser(int userId, string deletedByUsername)
    {
        // Soft delete - data preserved for audit
        _userRepository.DeleteUser(userId, deletedByUsername);
    }
    
    public void RestoreUser(int userId)
    {
        _userRepository.RestoreUser(userId);
    }
}
```

## Applying the Migration

To apply the soft delete columns to your database:

```powershell
# Preview the SQL that will be executed
dotnet ef migrations script --project ASI.Basecode.Data --startup-project ASI.Basecode.WebApp/ASI.Basecode.WebApp.csproj

# Apply the migration to the database
dotnet ef database update --project ASI.Basecode.Data --startup-project ASI.Basecode.WebApp/ASI.Basecode.WebApp.csproj
```

## Best Practices

### 1. **When to Use Soft Delete**
- User-generated content (users, profiles, addresses)
- Business-critical data (products, restaurants, staff)
- Data with relationships that shouldn't be broken
- When you need audit trails
- When recovery might be needed

### 2. **When to Use Hard Delete**
- Temporary data (shopping carts)
- Security tokens (after expiration)
- Test data
- Duplicate records
- When specifically required by regulations (GDPR "right to be forgotten")

### 3. **Never Delete**
- Financial transactions (orders, payments)
- Audit logs
- Legal documents
- Historical records

### 4. **Always Track Who Deleted**
```csharp
// Good - track who deleted
SoftDelete(entity, currentUser.Username);

// Bad - no audit trail
SoftDelete(entity, "System");
```

### 5. **Consider Cascade Behavior**
When soft deleting parent entities, consider whether child entities should also be soft deleted:

```csharp
public void DeleteRestaurant(int restaurantId, string deletedBy)
{
    var restaurant = GetDbSet<Restaurant>().Find(restaurantId);
    if (restaurant != null)
    {
        // Soft delete the restaurant
        SoftDelete(restaurant, deletedBy);
        
        // Also soft delete related staff
        var staff = GetDbSet<RestaurantStaff>()
            .Where(s => s.User.RestaurantStaff.StaffID == restaurantId);
        foreach (var staffMember in staff)
        {
            SoftDelete(staffMember, deletedBy);
        }
        
        UnitOfWork.SaveChanges();
    }
}
```

## Testing Soft Delete

```csharp
[TestClass]
public class SoftDeleteTests
{
    [TestMethod]
    public void SoftDelete_ShouldMarkEntityAsDeleted()
    {
        // Arrange
        var product = new Product { ProductID = 1, ProductName = "Test" };
        var repository = new ProductRepository(unitOfWork);
        
        // Act
        repository.SoftDelete(product, "testuser");
        
        // Assert
        Assert.IsTrue(product.IsDeleted);
        Assert.IsNotNull(product.DeletedDate);
        Assert.AreEqual("testuser", product.DeletedBy);
    }
    
    [TestMethod]
    public void GetActive_ShouldNotReturnDeletedEntities()
    {
        // Arrange
        var product = new Product { IsDeleted = true };
        
        // Act
        var activeProducts = repository.GetActive<Product>().ToList();
        
        // Assert
        Assert.IsFalse(activeProducts.Contains(product));
    }
}
```

## Database Schema Changes

The migration adds three columns to applicable tables:

- `IsDeleted` (bit/tinyint) - Default: false
- `DeletedDate` (datetime) - Nullable
- `DeletedBy` (nvarchar/longtext) - Nullable

## Performance Considerations

1. **Index on IsDeleted**: Consider adding an index for better query performance
2. **Archival Strategy**: Plan to archive old soft-deleted records periodically
3. **Query Filters**: Be aware that `IgnoreQueryFilters()` can impact performance

## Common Pitfalls to Avoid

1. ❌ **Don't forget to call SaveChanges**
   ```csharp
   SoftDelete(entity, "user");
   // Missing: UnitOfWork.SaveChanges();
   ```

2. ❌ **Don't use hard delete on entities with foreign key relationships without handling dependencies**
   
3. ❌ **Don't forget to use IgnoreQueryFilters when you need to query deleted records**

4. ❌ **Don't expose restore functionality to regular users** - Make it admin-only

## Questions?

If you have questions about when to use soft delete vs hard delete, or how to implement it for a specific entity, consult this guide or review the entity's business requirements.
