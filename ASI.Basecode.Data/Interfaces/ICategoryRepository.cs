using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Interfaces
{
    public interface ICategoryRepository
    {
        IQueryable<Category> GetAllCategories();
        IQueryable<Category> GetCategoryByID(int categoryID);
        void AddCategory(Category category);
        void UpdateCategory(Category category);
        void DeleteCategory(int categoryID);
    }
}
