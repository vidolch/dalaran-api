// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace JSONMockifyAPI.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using JSONMockifyAPI.Data.Models;
    using JSONMockifyAPI.Data.Repositories.Interfaces;
    using JSONMockifyAPI.Services.Data.Contracts;

    public class DataService<TIdentity, TEntity> : IDataService<TIdentity, TEntity>
        where TIdentity : class
        where TEntity : BaseModel
    {
        private IRepository<TIdentity, TEntity> repository;

        public DataService(IRepository<TIdentity, TEntity> repository)
        {
            this.repository = repository;
        }

        public Task AddOrUpdateAsync(TIdentity identity, TEntity entity)
        {
            return this.repository.AddOrUpdateAsync(identity, entity);
        }

        public Task<bool> DeleteAsync(TIdentity identity)
        {
            return this.repository.DeleteAsync(identity);
        }

        public Task<(IEnumerable<TEntity>, long)> GetAllAsync(Expression<Func<TEntity, bool>> predicate = null, int page = 0, int size = 20)
        {
            return this.repository.GetAllAsync(predicate, page, size);
        }

        public Task<TEntity> GetAsync(TIdentity identity)
        {
            return this.repository.GetAsync(identity);
        }

        public Task<bool> RecordExistsAsync(TIdentity identity)
        {
            return this.repository.RecordExistsAsync(identity);
        }
    }
}
