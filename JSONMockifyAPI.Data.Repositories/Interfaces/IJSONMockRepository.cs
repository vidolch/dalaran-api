// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace JSONMockifyAPI.Data.Repositories.Interfaces
{
    using System.Threading.Tasks;
    using JSONMockifyAPI.Data.Models;

    public interface IJSONMockRepository : IRepository<string, JSONMock>
    {
        Task<JSONMock> GetForMethodAsync(string id, HttpMethods method);
    }
}
