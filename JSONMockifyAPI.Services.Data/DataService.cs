﻿// Copyright (c) Vidol Chalamov.
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

    public class DataService<TEntity> : IDataService<TEntity>
        where TEntity : BaseModel
    {
        private IRepository<TEntity> repository;

        public DataService(IRepository<TEntity> repository)
        {
            this.repository = repository;
        }

        public Task AddOrUpdateAsync(TEntity entity)
        {
            return this.repository.AddOrUpdateAsync(entity);
        }

        public Task<bool> DeleteAsync(string identity)
        {
            return this.repository.DeleteAsync(identity);
        }

        public Task<(IEnumerable<TEntity>, long)> GetAllAsync(Expression<Func<TEntity, bool>> predicate = null, int page = 0, int size = 20)
        {
            return this.repository.GetAllAsync(predicate, page, size);
        }

        public Task<TEntity> GetAsync(string identity)
        {
            return this.repository.GetAsync(identity);
        }

        public Task<bool> RecordExistsAsync(string identity)
        {
            return this.repository.RecordExistsAsync(identity);
        }
    }
}
