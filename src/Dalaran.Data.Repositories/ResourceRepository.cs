// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace Dalaran.Data.Repositories
{
    using Dalaran.Data.Models;
    using Dalaran.Data.Repositories.Databases;
    using Dalaran.Data.Repositories.Interfaces;

    public class ResourceRepository : Repository<Resource>, IResourceRepository
    {
        public ResourceRepository(IDBRepository<Resource> dbRepository)
            : base(dbRepository)
        {
        }
    }
}
