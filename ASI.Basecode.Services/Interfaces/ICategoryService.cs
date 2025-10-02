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
        IQueryable<Category> GetAllCategories();
        Category GetCategoryByID(int categoryID);
        void AddCategory(CategoryViewModel category, int userID);
        void UpdateCategory(CategoryViewModel category, int userID);
        void DeleteCategory(int categoryID);
    }
}
