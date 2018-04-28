using JSONMockifyAPI.Data.Models;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace JSONMockifyAPI.Data.Repositories.Interfaces
{
    public interface IDBRepository<TEntity>
        where TEntity : BaseModel
    {
        TEntity Get(Guid id);

        TEntity Insert(TEntity entity);

        TEntity Update(TEntity entity);

        void Delete(TEntity entity);

        IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate, int page, int size);

        IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate);

        IQueryable<TEntity> GetAll();

        bool RecordExists(Guid id);
    }
}
