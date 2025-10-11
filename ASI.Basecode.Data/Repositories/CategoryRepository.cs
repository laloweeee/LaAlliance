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

        /// <summary>
        /// Get all product categories.
        /// </summary>
        /// <returns></returns>
        public IQueryable<ProductCategory> GetAllCategories()
        {
            return _dbContext.ProductCategories.AsQueryable();
        }

        /// <summary>
        /// Get a category by its ID.
        /// </summary>
        /// <param name="categoryID"></param>
        /// <returns></returns>
        public ProductCategory GetCategoryByID(int categoryID)
        {
            return _dbContext.ProductCategories.FirstOrDefault(c => c.CategoryID == categoryID);
        }

        /// <summary>
        /// Add a new product category.
        /// </summary>
        /// <param name="category"></param>
        public void AddCategory(ProductCategory category)
        {
            _dbContext.ProductCategories.Add(category);
            UnitOfWork.SaveChanges();
        }

        /// <summary>
        /// Update an existing product category.
        /// </summary>
        /// <param name="category"></param>
        public void UpdateCategory(ProductCategory category)
        {
            _dbContext.ProductCategories.Update(category);
            UnitOfWork.SaveChanges();
        }

        /// <summary>
        /// Delete a product category by its ID.
        /// </summary>
        /// <param name="categoryID"></param>
        public void DeleteCategory(int categoryID)
        {
            var category = GetCategoryByID(categoryID);
            _dbContext.ProductCategories.Remove(category);
            UnitOfWork.SaveChanges();
        }
    }
}