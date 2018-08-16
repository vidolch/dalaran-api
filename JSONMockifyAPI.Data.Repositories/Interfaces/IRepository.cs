// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace JSONMockifyAPI.Data.Repositories.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using JSONMockifyAPI.Data.Models;

    public interface IRepository<TIdentity, TEntity>
        where TIdentity : class
        where TEntity : BaseModel
    {
        Task<(IEnumerable<TEntity>, long)> GetAllAsync(Expression<Func<TEntity, bool>> predicate = default, int page = default, int size = 20);

        Task<TEntity> GetAsync(TIdentity identity);

        Task AddOrUpdateAsync(TIdentity identity, TEntity entity);

        Task<bool> DeleteAsync(TIdentity identity);

        Task<bool> RecordExistsAsync(TIdentity identity);
    }
}
