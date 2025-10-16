using ASI.Basecode.Data.Models;
using System.Linq;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IBaseRepository
    {
        void SoftDelete<T>(T entity, string deletedBy) where T : class, ISoftDeletable;
        void HardDelete<T>(T entity) where T : class;
        void Restore<T>(T entity) where T : class, ISoftDeletable;
        IQueryable<T> GetActive<T>() where T : class, ISoftDeletable;
        IQueryable<T> GetAllIncludingDeleted<T>() where T : class, ISoftDeletable;
        void SaveChanges();
    }
}