using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IProductRepository
    {
        void AddProduct(Product product);
        void UpdateProduct(Product product);
        void DeleteProduct(int productID);
        IQueryable<Product> GetProducts();
        IQueryable<Product> GetProductByID(int productID);
        IQueryable<Product> GetProductsByCategoryID(int categoryID);
        IQueryable<Product> GetActiveProducts();
        void UpdateProductCustomizations(int productID, List<CustomizationGroup> customizations);
        IQueryable<Product> GetProductsByFilter(Expression<Func<Product, bool>> filter);
        IQueryable<Product> SearchProduct(string searchTerm);
    }
}
