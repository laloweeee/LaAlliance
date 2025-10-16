using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IProductService
    {
        IQueryable<Product> GetAllProducts();
        ProductViewModel GetProductByID(int productID);
        IQueryable<Product> GetProductsByCategoryID(int categoryID);
        List<ProductViewModel> GetActiveProducts();
        Task AddProduct(ProductViewModel model);
        Task EditProduct(ProductViewModel model);
    }
}