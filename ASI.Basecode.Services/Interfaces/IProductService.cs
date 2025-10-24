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
        /// <summary>
        /// Get all products
        /// </summary>
        /// <returns></returns>
        IQueryable<Product> GetAllProducts();
        
        /// <summary>
        /// Get product by ID
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        ProductViewModel GetProductByID(int productID);
        
        /// <summary>
        /// Get active products
        /// </summary>
        /// <returns></returns>
        List<ProductViewModel> GetActiveProducts();

        /// <summary>
        /// Get deleted products
        /// </summary>
        /// <returns></returns>
        List<ProductViewModel> GetDeletedProducts();

        /// <summary>
        /// Add product
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task AddProduct(ProductViewModel model);

        /// <summary>
        /// Edit product
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task EditProduct(ProductViewModel model);

        /// <summary>
        /// Delete product
        /// </summary>
        /// <param name="productID"></param>
        /// <param name="deletedBy"></param>
        void DeleteProduct(int productID, string deletedBy);

        /// <summary>
        /// Recover product
        /// </summary>
        /// <param name="productID"></param>
        void RecoverProduct(int productID);

        /// <summary>
        /// Permanently delete product
        /// </summary>
        /// <param name="productID"></param>
        void PermanentDelete(int productID);

        // Methods for dashboard
        Task<List<MostSellingItemViewModel>> GetMostSellingItemsAsync(int count = 5);
        Task<List<MostFavoriteItemViewModel>> GetMostFavoriteItemsAsync(int count = 5);
    }
}