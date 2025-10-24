using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASI.Basecode.Data.Repositories
{
    public class CustomerProductFavoritesRepository : BaseRepository, ICustomerProductFavoritesRepository
    {
        public CustomerProductFavoritesRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IQueryable<CustomerProductFavorites> GetCustomerProductFavorites()
        {
            if (this.Context == null)
            {
                throw new InvalidOperationException("Database context is not available.");
            }

            return this.Context.CustomerProductFavorites
                .Include(f => f.Product)
                .Where(f => !f.IsDeleted);
        }

        public Dictionary<int, int> GetFavoriteCountsByProduct()
        {
            if (this.Context == null)
            {
                throw new InvalidOperationException("Database context is not available.");
            }

            return this.Context.CustomerProductFavorites
                .Where(f => !f.IsDeleted)
                .GroupBy(f => f.ProductID)
                .Select(g => new
                {
                    ProductId = g.Key,
                    Count = g.Count()
                })
                .ToDictionary(x => x.ProductId, x => x.Count);
        }

        public void AddFavorite(CustomerProductFavorites favorite)
        {
            if (this.Context == null)
            {
                throw new InvalidOperationException("Database context is not available.");
            }

            favorite.DateAdded = DateTime.UtcNow;
            this.Context.CustomerProductFavorites.Add(favorite);
            this.Context.SaveChanges();
        }

        public void RemoveFavorite(CustomerProductFavorites favorite)
        {
            if (this.Context == null)
            {
                throw new InvalidOperationException("Database context is not available.");
            }

            var existingFavorite = this.Context.CustomerProductFavorites
                .FirstOrDefault(f => f.UserID == favorite.UserID && f.ProductID == favorite.ProductID && !f.IsDeleted);
            
            if (existingFavorite != null)
            {
                this.Context.CustomerProductFavorites.Remove(existingFavorite);
                this.Context.SaveChanges();
            }
        }

        public bool IsProductFavoritedByUser(int userId, int productId)
        {
            if (this.Context == null)
            {
                throw new InvalidOperationException("Database context is not available.");
            }

            return this.Context.CustomerProductFavorites
                .Any(f => f.UserID == userId && f.ProductID == productId && !f.IsDeleted);
        }

        public IQueryable<CustomerProductFavorites> GetUserFavorites(int userId)
        {
            if (this.Context == null)
            {
                throw new InvalidOperationException("Database context is not available.");
            }

            return this.Context.CustomerProductFavorites
                .Include(f => f.Product)
                .Where(f => f.UserID == userId && !f.IsDeleted);
        }
    }
}