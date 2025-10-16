# Soft Delete Quick Reference

## ✅ Implementation Summary

### What Was Done
1. ✅ **BaseEntity** now implements `ISoftDeletable` interface with:
   - `IsDeleted` (bool)
   - `DeletedDate` (DateTime?)
   - `DeletedBy` (string?)

2. ✅ **13 Entities** updated to inherit from `BaseEntity`:
   - User, UserProfile, UserAddress
   - Restaurant, RestaurantAddress, RestaurantStaff
   - Product, ProductCategory
   - Address
   - RestaurantPromotions, PromotionCodes
   - DeliveryPolicy
   - CustomerProductFavorites

3. ✅ **Global Query Filter** automatically excludes soft-deleted records from all queries

4. ✅ **BaseRepository** enhanced with 5 new methods:
   - `SoftDelete<TEntity>(entity, deletedBy)`
   - `HardDelete<TEntity>(entity)`
   - `GetActive<TEntity>()`
   - `GetAllIncludingDeleted<TEntity>()`
   - `Restore<TEntity>(entity)`

5. ✅ **EF Core Migration** created: `AddSoftDeleteSupport`

## 🚀 Quick Start

### Apply Migration to Database
```powershell
dotnet ef database update --project ASI.Basecode.Data --startup-project ASI.Basecode.WebApp/ASI.Basecode.WebApp.csproj
```

### Usage in Repository
```csharp
public class YourRepository : BaseRepository
{
    // Soft delete (recommended for most entities)
    public void Delete(YourEntity entity, string username)
    {
        SoftDelete(entity, username);
        UnitOfWork.SaveChanges();
    }
    
    // Get only active records (IsDeleted = false)
    public IEnumerable<YourEntity> GetAll()
    {
        return GetActive<YourEntity>().ToList();
    }
    
    // Restore deleted record
    public void RestoreEntity(int id)
    {
        var entity = GetAllIncludingDeleted<YourEntity>()
            .FirstOrDefault(e => e.Id == id);
        if (entity?.IsDeleted == true)
        {
            Restore(entity);
            UnitOfWork.SaveChanges();
        }
    }
}
```

## 📋 Entity Classification

| Strategy | Entities | Reason |
|----------|----------|---------|
| **Soft Delete** | User, Product, Restaurant, etc. | Audit trail, recovery, data integrity |
| **Hard Delete** | Cart, CartItem, VerificationTokens | Temporary data, security |
| **Never Delete** | Order, OrderItems, PaymentLog | Compliance, legal requirements |

## 🎯 Common Patterns

```csharp
// Pattern 1: Soft delete
var product = GetDbSet<Product>().Find(id);
SoftDelete(product, currentUser.Username);
UnitOfWork.SaveChanges();

// Pattern 2: Query active only (default)
var products = GetDbSet<Product>().ToList(); // Automatically excludes deleted

// Pattern 3: Query including deleted
var allProducts = GetDbSet<Product>().IgnoreQueryFilters().ToList();

// Pattern 4: Query only deleted
var deleted = GetDbSet<Product>()
    .IgnoreQueryFilters()
    .Where(p => p.IsDeleted)
    .ToList();

// Pattern 5: Restore
var product = GetAllIncludingDeleted<Product>().Find(id);
Restore(product);
UnitOfWork.SaveChanges();
```

## ⚠️ Important Notes

1. **Always call `UnitOfWork.SaveChanges()`** after delete/restore operations
2. **Global filter is automatic** - all queries exclude deleted records by default
3. **Use `IgnoreQueryFilters()`** when you need to query deleted records
4. **Track who deleted** - always pass username/userId to `SoftDelete()`
5. **Consider cascade deletes** - decide if child entities should also be deleted

## 📖 Full Documentation
See `SOFT_DELETE_GUIDE.md` for comprehensive documentation, examples, and best practices.
