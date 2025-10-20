using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Interfaces
{
    public interface ICategoryService
    {
        /// <summary>
        /// Get all categories
        /// </summary>
        /// <returns></returns>
        List<ProductCategory> GetAllCategories();

        /// <summary>
        /// Get deleted product categories
        /// </summary>
        /// <returns></returns>
        List<ProductCategory> GetDeletedProductCategories();

        /// <summary>
        /// Get category by ID
        /// </summary>
        /// <param name="categoryID"></param>
        /// <returns></returns>
        ProductCategory GetCategoryByID(int categoryID);

        /// <summary>
        /// Add category
        /// </summary>
        /// <param name="category"></param>
        void AddCategory(CategoryViewModel category);

        /// <summary>
        /// Update category
        /// </summary>
        /// <param name="category"></param>
        void UpdateCategory(CategoryViewModel category);

        /// <summary>
        /// Delete category
        /// </summary>
        /// <param name="categoryID"></param>
        /// <param name="deletedBy"></param>
        void DeleteCategory(int categoryID, string deletedBy);

        /// <summary>
        /// Recover category
        /// </summary>
        /// <param name="categoryID"></param>
        void RecoverCategory(int categoryID);
        
        /// <summary>
        /// Permanently delete category
        /// </summary>
        /// <param name="categoryID"></param>
        void PermanentDeleteCategory(int categoryID);
    }
}
