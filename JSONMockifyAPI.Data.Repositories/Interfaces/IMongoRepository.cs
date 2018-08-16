﻿// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace JSONMockifyAPI.Data.Repositories.Interfaces
{
    using JSONMockifyAPI.Data.Models;

    public interface IMongoRepository<TIdentity, TEntity> : IRepository<TIdentity, TEntity>
        where TIdentity : class
        where TEntity : BaseModel
    {
    }
}
