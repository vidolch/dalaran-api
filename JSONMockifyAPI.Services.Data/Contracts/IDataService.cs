// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace JSONMockifyAPI.Services.Data.Contracts
{
    using System;
    using System.Collections.Generic;
    using JSONMockifyAPI.Data.Models;

    public interface IDataService<TEntity>
        where TEntity : BaseModel
    {
        IEnumerable<TEntity> GetAll();

        TEntity Create(TEntity entity);

        TEntity Get(Guid id);

        void Delete(TEntity entity);

        void Delete(Guid id);

        TEntity Update(TEntity entity);

        bool RecordExists(Guid id);
    }
}