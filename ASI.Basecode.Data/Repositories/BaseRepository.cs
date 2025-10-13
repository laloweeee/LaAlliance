using ASI.Basecode.Data;
using ASI.Basecode.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace ASI.Basecode.Data.Repositories
{
    public class BaseRepository
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
    }
}
