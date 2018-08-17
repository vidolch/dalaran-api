// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace JSONMockifyAPI.Data.Repositories
{
    using System.Threading.Tasks;
    using JSONMockifyAPI.Data.Models;
    using JSONMockifyAPI.Data.Repositories.Databases;
    using JSONMockifyAPI.Data.Repositories.Interfaces;

    public class JSONMockRepository : Repository<string, JSONMock>, IJSONMockRepository
    {
        public JSONMockRepository(IDBRepository<string, JSONMock> dbInstance)
            : base(dbInstance)
        {
        }

        public async Task<JSONMock> GetForMethodAsync(string id, HttpMethods method)
        {
            var result = await this.GetAsync(id);

            if (result.HttpMethod != method)
            {
                return null;
            }

            return result;
        }
    }
}
