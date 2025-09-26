using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Repositories
{
    public class CategoryRepository : BaseRepository, ICategoryRepository
    {
        public CategoryRepository(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        public IQueryable<Category> GetAllCategories()
        {
            return GetDbSet<Category>();
        }

        public IQueryable<Category> GetCategoryByID(int categoryID)
        {
            return GetDbSet<Category>().Where(x => x.CategoryID == categoryID);
        }

        public void AddCategory(Category category)
        {
            category.CreatedTime = DateTime.UtcNow;
            category.UpdatedTime = DateTime.UtcNow;
            GetDbSet<Category>().Add(category);
            UnitOfWork.SaveChanges();
        }

        public void UpdateCategory(Category category)
        {
            category.UpdatedTime = DateTime.UtcNow;
            GetDbSet<Category>().Update(category);
            UnitOfWork.SaveChanges();
        }

        public void DeleteCategory(int categoryID)
        {
            var category = GetCategoryByID(categoryID).FirstOrDefault();
            if (category != null)
            {
                GetDbSet<Category>().Remove(category);
                UnitOfWork.SaveChanges();
            }
        }
    }
}
