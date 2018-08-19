// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace JSONMockifyAPI.Data.Repositories
{
    using JSONMockifyAPI.Data.Models;
    using JSONMockifyAPI.Data.Repositories.Databases;
    using JSONMockifyAPI.Data.Repositories.Interfaces;

    public class CollectionRepository : Repository<Collection>, ICollectionRepository
    {
        public CollectionRepository(IDBRepository<Collection> dbRepository)
            : base(dbRepository)
        {
        }
    }
}
