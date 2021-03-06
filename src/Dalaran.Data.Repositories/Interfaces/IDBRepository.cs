﻿// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace Dalaran.Data.Repositories.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Dalaran.Data.Models;

    public interface IDBRepository<TEntity>
        where TEntity : BaseModel
    {
        Task<(IEnumerable<TEntity>, long)> GetAllAsync(Expression<Func<TEntity, bool>> predicate = default, int page = default, int size = 20);

        Task<TEntity> GetAsync(string identity);

        Task AddOrUpdateAsync(TEntity entity);

        Task<bool> DeleteAsync(string identity);

        Task<bool> RecordExistsAsync(string identity);

        Task<bool> RecordExistsAsync(Expression<Func<TEntity, bool>> predicate = default);
    }
}
