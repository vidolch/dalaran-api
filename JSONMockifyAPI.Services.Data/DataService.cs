// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace JSONMockifyAPI.Services.Data
{
    using System;
    using System.Collections.Generic;
    using JSONMockifyAPI.Data.Models;
    using JSONMockifyAPI.Data.Repositories.Interfaces;
    using JSONMockifyAPI.Services.Data.Contracts;

    public class DataService<TEntity> : IDataService<TEntity>
        where TEntity : BaseModel
    {
        private IRepository<TEntity> repository;

        public DataService(IRepository<TEntity> repository)
        {
            this.repository = repository;
        }

        public TEntity Create(TEntity entity)
        {
            return this.repository.Insert(entity);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return this.repository.GetAll();
        }

        public TEntity Get(Guid id)
        {
            return this.repository.Get(id);
        }

        public void Delete(TEntity entity)
        {
            this.repository.Delete(entity);
        }

        public void Delete(Guid id)
        {
            TEntity entity = this.Get(id);
            if (entity != null)
            {
                this.repository.Delete(entity);
            }
        }

        public TEntity Update(TEntity entity)
        {
            return this.repository.Update(entity);
        }

        public bool RecordExists(Guid id)
        {
            return this.repository.RecordExists(id);
        }
    }
}
