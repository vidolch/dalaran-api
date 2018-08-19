// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace JSONMockifyAPI.Services.Data
{
    using JSONMockifyAPI.Data.Models;
    using JSONMockifyAPI.Data.Repositories.Interfaces;
    using JSONMockifyAPI.Services.Data.Contracts;

    public class CollectionService : DataService<Collection>, ICollectionService
    {
        public CollectionService(IRepository<Collection> repository)
            : base(repository)
        {
        }
    }
}
