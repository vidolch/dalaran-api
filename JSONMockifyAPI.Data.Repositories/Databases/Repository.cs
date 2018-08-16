// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace JSONMockifyAPI.Data.Repositories.Databases
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
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

        public void Delete(TEntity entity)
        {
            this.dbRepository.Delete(entity);
        }

        public TEntity Get(Guid id)
        {
            return this.dbRepository.Get(id);
        }

        public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate, int page, int size)
        {
            return this.dbRepository.GetAll(predicate, page, size);
        }

        public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate)
        {
            return this.dbRepository.GetAll(predicate);
        }

        public IQueryable<TEntity> GetAll()
        {
            return this.dbRepository.GetAll();
        }

        public TEntity Insert(TEntity entity)
        {
            return this.dbRepository.Insert(entity);
        }

        public bool RecordExists(Guid id)
        {
            return this.dbRepository.RecordExists(id);
        }

        public TEntity Update(TEntity entity)
        {
            return this.dbRepository.Update(entity);
        }
    }
}
