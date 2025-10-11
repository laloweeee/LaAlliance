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
        private readonly AsiBasecodeDBContext _dbContext;
        public ProductRepository(IUnitOfWork unitOfWork, AsiBasecodeDBContext dbContext) : base(unitOfWork)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Add a new product.
        /// </summary>
        /// <param name="product"></param>
        public void AddProduct(Product product)
        {
            _dbContext.Products.Add(product);
            UnitOfWork.SaveChanges();
        }

        /// <summary>
        /// Update an existing product.
        /// </summary>
        /// <param name="product"></param>
        public void UpdateProduct(Product product)
        {
            _dbContext.Products.Update(product);
            UnitOfWork.SaveChanges();
        }

        /// <summary>
        /// Delete a product by its ID.
        /// </summary>
        /// <param name="productID"></param>
        public void DeleteProduct(int productID)
        {
            var product = _dbContext.Products.Find(productID);
            if (product != null)
            {
                _dbContext.Products.Remove(product);
                UnitOfWork.SaveChanges();
            }
        }

        /// <summary>
        /// Get all products with their categories and option groups.
        /// </summary>
        /// <returns></returns>
        public IQueryable<Product> GetProducts()
        {
            return _dbContext.Products.Include(p => p.ProductCategory).Include(p => p.ProductOptionGroup).ThenInclude(g => g.ProductOptionItems);
        }

        /// <summary>
        /// Get a product by its ID with its category and option groups.
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public IQueryable<Product> GetProductByID(int productID)
        {
            return _dbContext.Products.Include(p => p.ProductCategory).Include(p => p.ProductOptionGroup).ThenInclude(g => g.ProductOptionItems).Where(x => x.ProductID == productID);
        }

        /// <summary>
        /// Get products by category ID with their categories and option groups.
        /// </summary>
        /// <param name="categoryID"></param>
        /// <returns></returns>
        public IQueryable<Product> GetProductsByCategoryID(int categoryID)
        {
            return _dbContext.Products.Include(p => p.ProductCategory).Include(p => p.ProductOptionGroup).ThenInclude(g => g.ProductOptionItems).Where(x => x.CategoryID == categoryID);
        }

        /// <summary>
        /// Get all active products.
        /// </summary>
        /// <returns></returns>
        public IQueryable<Product> GetActiveProducts()
        {
            return GetProducts().Where(x => x.IsActive);
        }

        /// <summary>
        /// Update product customizations (option groups and items).
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="productOptionGroups"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void UpdateProductCustomizations(int productId, List<ProductOptionGroup> productOptionGroups)
        {
            // TODO: Implement this method to update product customizations
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get products based on a filter expression.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IQueryable<Product> GetProductsByFilter(Expression<Func<Product, bool>> filter)
        {
            return GetDbSet<Product>()
                    .Include(p => p.ProductCategory)
                    .Include(p => p.ProductOptionGroup)
                    .ThenInclude(g => g.ProductOptionItems)
                    .Where(filter);
        }

        /// <summary>
        /// Search products by name or description.
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        public IQueryable<Product> SearchProduct(string searchTerm)
        {
            return GetDbSet<Product>()
                    .Include(p => p.ProductCategory)
                    .Include(p => p.ProductOptionGroup)
                    .ThenInclude(g => g.ProductOptionItems)
                    .Where(p => p.ProductName.Contains(searchTerm) || p.ProductDescription.Contains(searchTerm));
        }
    }
}