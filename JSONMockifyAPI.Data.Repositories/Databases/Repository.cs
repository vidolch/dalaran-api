// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace JSONMockifyAPI.Data.Repositories.Databases
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
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

        public virtual Task AddOrUpdateAsync(TIdentity identity, TEntity entity)
        {
            entity.ID = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);
            entity.CreatedTimestamp = DateTimeOffset.Now;

            return this.dbRepository.AddOrUpdateAsync(identity, entity);
        }

        public virtual Task<bool> DeleteAsync(TIdentity identity)
        {
            return this.dbRepository.DeleteAsync(identity);
        }

        public virtual Task<(IEnumerable<TEntity>, long)> GetAllAsync(Expression<Func<TEntity, bool>> predicate = null, int page = 0, int size = 20)
        {
            return this.dbRepository.GetAllAsync(predicate, page, size);
        }

        public virtual Task<TEntity> GetAsync(TIdentity identity)
        {
            return this.dbRepository.GetAsync(identity);
        }

        public virtual Task<bool> RecordExistsAsync(TIdentity identity)
        {
            return this.dbRepository.RecordExistsAsync(identity);
        }
    }
}
