// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace JSONMockifyAPI.Data.Repositories.Databases
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using JSONMockifyAPI.Data.Models;
    using JSONMockifyAPI.Data.Repositories.Interfaces;

    public abstract class Repository<TIdentity, TEntity> : IRepository<TIdentity, TEntity>
        where TIdentity : class
        where TEntity : BaseModel
    {
        private IDBRepository<TIdentity, TEntity> dbRepository;

        public Repository(IDBRepository<TIdentity, TEntity> dbRepository)
        {
            this.dbRepository = dbRepository;
        }

        public Task AddOrUpdateAsync(TIdentity identity, TEntity entity)
        {
            return this.dbRepository.AddOrUpdateAsync(identity, entity);
        }

        public Task<bool> DeleteAsync(TIdentity identity)
        {
            return this.dbRepository.DeleteAsync(identity);
        }

        public Task<(IEnumerable<TEntity>, long)> GetAllAsync(Expression<Func<TEntity, bool>> predicate = null, int page = 0, int size = 20)
        {
            return this.dbRepository.GetAllAsync(predicate, page, size);
        }

        public Task<TEntity> GetAsync(TIdentity identity)
        {
            return this.dbRepository.GetAsync(identity);
        }

        public Task<bool> RecordExistsAsync(TIdentity identity)
        {
            return this.dbRepository.RecordExistsAsync(identity);
        }
    }
}
