using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Repositories
{
    public class ProductRepository : BaseRepository, IProductRepository
    {
        public ProductRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void AddProduct(Product product)
        {

        }

        public void UpdateProduct(Product product)
        {

        }

        public void DeleteProduct(Product product)
        {

        }

        public IQueryable<Product> GetProducts()
        {
            return GetDbSet<Product>();
        }

        public IQueryable<Product> GetProductByID(int productID)
        {
            return GetDbSet<Product>().Where(x => x.ProductID == productID);
        }
    }
}
