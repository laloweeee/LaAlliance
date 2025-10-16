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
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Update an existing product.
        /// </summary>
        /// <param name="product"></param>
        public void UpdateProduct(Product product)
        {
            _dbContext.Products.Update(product);
            _dbContext.SaveChanges();
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
                _dbContext.SaveChanges();
            }
        }

        /// <summary>
        /// Get all products with their categories and option groups.
        /// </summary>
        /// <returns></returns>
        public IQueryable<Product> GetProducts()
        {
            return _dbContext.Products
                .Include(p => p.ProductCategory)
                .Include(p => p.ProductOptionGroup)
                    .ThenInclude(g => g.ProductOptionItems);
        }
    }
}