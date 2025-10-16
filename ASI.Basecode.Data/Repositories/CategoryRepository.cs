using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using System.Linq;

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
        /// Add a new product category.
        /// </summary>
        /// <param name="category"></param>
        public void AddCategory(ProductCategory category)
        {
            _dbContext.ProductCategories.Add(category);
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Update an existing product category.
        /// </summary>
        /// <param name="category"></param>
        public void UpdateCategory(ProductCategory category)
        {
            _dbContext.ProductCategories.Update(category);
            _dbContext.SaveChanges();
        }
    }
}