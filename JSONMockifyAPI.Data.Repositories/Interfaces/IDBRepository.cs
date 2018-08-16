// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace JSONMockifyAPI.Data.Repositories.Interfaces
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using JSONMockifyAPI.Data.Models;

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
