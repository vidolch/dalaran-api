// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace Dalaran.Data.Mongo
{
    using Dalaran.Data.Models;
    using Dalaran.Data.Repositories.Interfaces;

    public interface IMongoRepository<TIdentity, TEntity> : IRepository<TEntity>
        where TEntity : BaseModel
    {
    }
}
