using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace ASI.Basecode.Data.Repositories
{
    public class ProductRepository : BaseRepository, IProductRepository
    {

        public ProductRepository(IUnitOfWork unitOfWork, AsiBasecodeDBContext dbContext) : base(unitOfWork)
        {
        }

        public void AddProduct(Product product)
        {
            GetDbSet<Product>().Add(product);
            UnitOfWork.SaveChanges();
        }

        public void UpdateProduct(Product product)
        {
            GetDbSet<Product>().Update(product);
            UnitOfWork.SaveChanges();
        }

        public void DeleteProduct(int productID)
        {
            var product = GetDbSet<Product>().Find(productID);
            if (product != null)
            {
                GetDbSet<Product>().Remove(product);
                UnitOfWork.SaveChanges();
            }
        }

        public IQueryable<Product> GetProducts()
        {
            return GetDbSet<Product>()
                    .Include(p => p.Category)
                    .Include(p => p.CustomizationGroups)
                    .ThenInclude(g => g.CustomizationOptions);
        }

        public IQueryable<Product> GetProductByID(int productID)
        {
            return GetDbSet<Product>()
                    .Include(p => p.Category)
                    .Include(p => p.CustomizationGroups)
                    .ThenInclude(g => g.CustomizationOptions)
                    .Where(x => x.ProductID == productID);
        }

        public IQueryable<Product> GetProductsByCategoryID(int categoryID)
        {
            return GetDbSet<Product>()
                    .Include(p => p.Category)
                    .Include(p => p.CustomizationGroups)
                    .ThenInclude(g => g.CustomizationOptions)
                    .Where(x => x.CategoryID == categoryID);
        }

        public IQueryable<Product> GetActiveProducts()
        {
            return GetProducts().Where(x => x.IsActive);
        }

        public void UpdateProductCustomizations(int productId, List<CustomizationGroup> customizationGroups)
        {
            // TODO: Implement this method to update product customizations
            throw new NotImplementedException();
        }

        public IQueryable<Product> GetProductsByFilter(Expression<Func<Product, bool>> filter)
        {
            return GetDbSet<Product>()
                    .Include(p => p.Category)
                    .Include(p => p.CustomizationGroups)
                    .ThenInclude(g => g.CustomizationOptions)
                    .Where(filter);
        }

        public IQueryable<Product> SearchProduct(string searchTerm)
        {
            return GetDbSet<Product>()
                    .Include(p => p.Category)
                    .Include(p => p.CustomizationGroups)
                    .ThenInclude(g => g.CustomizationOptions)
                    .Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm));
        }
    }
}
