using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Repositories
{
    public class CategoryRepository : BaseRepository, ICategoryRepository
    {
        private readonly AsiBasecodeDBContext _dbContext;
        public CategoryRepository(IUnitOfWork unitOfWork, AsiBasecodeDBContext dbContext) : base(unitOfWork)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Category> GetAllCategories()
        {
            // return GetDbSet<Category>();
            return _dbContext.Categories;
        }

        public Category GetCategoryByID(int categoryID)
        {
            return _dbContext.Categories.FirstOrDefault(c => c.CategoryID == categoryID);
        }

        public void AddCategory(Category category)
        {
            category.CreatedTime = DateTime.UtcNow;
            category.UpdatedTime = DateTime.UtcNow;
            _dbContext.Categories.Add(category);
            UnitOfWork.SaveChanges();
        }

        public void UpdateCategory(Category category)
        {
            _dbContext.Categories.Update(category);
            UnitOfWork.SaveChanges();
        }

        public void DeleteCategory(int categoryID)
        {
            var category = GetCategoryByID(categoryID);
            if (category != null)
            {
                GetDbSet<Category>().Remove(category);
                UnitOfWork.SaveChanges();
            }
        }
    }
}
