// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace Dalaran.Data.Repositories
{
    using System.Threading.Tasks;
    using Dalaran.Data.Models;
    using Dalaran.Data.Repositories.Databases;
    using Dalaran.Data.Repositories.Interfaces;

    public class RequestRepository : Repository<Request>, IRequestRepository
    {
        public RequestRepository(IDBRepository<Request> dbInstance)
            : base(dbInstance)
        {
        }

        public async Task<Request> GetForMethodAsync(string id, HttpMethods method)
        {
            var result = await this.GetAsync(id);

            if (result == null || result.HttpMethod != method)
            {
                return null;
            }

            return result;
        }
    }
}
