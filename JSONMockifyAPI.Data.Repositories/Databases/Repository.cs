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

    public abstract class Repository<TEntity> : IRepository<TEntity>
        where TEntity : BaseModel
    {
        private IDBRepository<TEntity> dbRepository;

        public Repository(IDBRepository<TEntity> dbRepository)
        {
            this.dbRepository = dbRepository;
        }

        public virtual Task AddOrUpdateAsync(TEntity entity)
        {
            entity.ID = entity.ID ?? Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);
            entity.CreatedTimestamp = DateTimeOffset.Now;

            return this.dbRepository.AddOrUpdateAsync(entity);
        }

        public virtual Task<bool> DeleteAsync(string identity)
        {
            return this.dbRepository.DeleteAsync(identity);
        }

        public virtual Task<(IEnumerable<TEntity>, long)> GetAllAsync(Expression<Func<TEntity, bool>> predicate = null, int page = 0, int size = 20)
        {
            return this.dbRepository.GetAllAsync(predicate, page, size);
        }

        public virtual Task<TEntity> GetAsync(string identity)
        {
            return this.dbRepository.GetAsync(identity);
        }

        public virtual Task<bool> RecordExistsAsync(string identity)
        {
            return this.dbRepository.RecordExistsAsync(identity);
        }
    }
}
