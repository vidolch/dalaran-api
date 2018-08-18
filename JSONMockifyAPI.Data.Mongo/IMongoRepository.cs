// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace JSONMockifyAPI.Data.Mongo
{
    using JSONMockifyAPI.Data.Models;
    using JSONMockifyAPI.Data.Repositories.Interfaces;

    public interface IMongoRepository<TIdentity, TEntity> : IRepository<TEntity>
        where TEntity : BaseModel
    {
    }
}
