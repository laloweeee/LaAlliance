using ASI.Basecode.Data;
using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace ASI.Basecode.Data.Repositories
{
    /// <summary>
    /// Base repository with soft delete support
    /// </summary>
    public class BaseRepository : IBaseRepository
    {
        protected IUnitOfWork UnitOfWork { get; set; }

        protected AsiBasecodeDBContext Context => (AsiBasecodeDBContext)UnitOfWork.Database;

        public BaseRepository(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null) throw new ArgumentNullException(nameof(unitOfWork));
            UnitOfWork = unitOfWork;
        }

        /// <summary>
        /// Get the DbSet for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        protected virtual DbSet<TEntity> GetDbSet<TEntity>() where TEntity : class
        {
            return Context.Set<TEntity>();
        }

        /// <summary>
        /// Set the entity state for the specified entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="entityState"></param>
        protected virtual void SetEntityState(object entity, EntityState entityState)
        {
            Context.Entry(entity).State = entityState;
        }

        /// <summary>
        /// Soft delete an entity by marking it as deleted
        /// </summary>
        /// <typeparam name="TEntity">Entity type that implements ISoftDeletable</typeparam>
        /// <param name="entity">The entity to soft delete</param>
        /// <param name="deletedBy">Username or ID of the user performing the deletion</param>
        public virtual void SoftDelete<TEntity>(TEntity entity, string deletedBy) where TEntity : class, ISoftDeletable
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            
            entity.IsDeleted = true;
            entity.DeletedDate = DateTime.UtcNow;
            entity.DeletedBy = deletedBy;

            Context.Update(entity);
            UnitOfWork.SaveChanges();
        }

        /// <summary>
        /// Hard delete an entity
        /// Use with caution - this is irreversible
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="entity">The entity to hard delete</param>
        public virtual void HardDelete<TEntity>(TEntity entity) where TEntity : class
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            Context.Remove(entity);
            UnitOfWork.SaveChanges();
        }

        /// <summary>
        /// Get all active (non-deleted) entities
        /// </summary>
        /// <typeparam name="TEntity">Entity type that implements ISoftDeletable</typeparam>
        /// <returns>Queryable of active entities</returns>
        public virtual IQueryable<TEntity> GetActive<TEntity>() where TEntity : class, ISoftDeletable
        {
            return Context.Set<TEntity>().Where(x => !x.IsDeleted);
        }

        /// <summary>
        /// Get all entities including soft-deleted ones
        /// Use IgnoreQueryFilters to bypass the global soft delete filter
        /// </summary>
        /// <typeparam name="TEntity">Entity type that implements ISoftDeletable</typeparam>
        /// <returns>Queryable of all entities including deleted ones</returns>
        public virtual IQueryable<TEntity> GetAllIncludingDeleted<TEntity>() where TEntity : class, ISoftDeletable
        {
            return Context.Set<TEntity>().IgnoreQueryFilters();
        }

        /// <summary>
        /// Restore a soft-deleted entity
        /// </summary>
        /// <typeparam name="TEntity">Entity type that implements ISoftDeletable</typeparam>
        /// <param name="entity">The entity to restore</param>
        public virtual void Restore<TEntity>(TEntity entity) where TEntity : class, ISoftDeletable
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            
            entity.IsDeleted = false;
            entity.DeletedDate = null;
            entity.DeletedBy = null;

            Context.Update(entity);
            UnitOfWork.SaveChanges();
        }

        /// <summary>
        /// Save changes to the database
        /// </summary>
        public virtual void SaveChanges()
        {
            UnitOfWork.SaveChanges();
        }
    }
}
