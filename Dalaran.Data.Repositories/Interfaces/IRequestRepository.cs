// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace Dalaran.Data.Repositories.Interfaces
{
    using System.Threading.Tasks;
    using Dalaran.Data.Models;

    public interface IRequestRepository : IRepository<Request>
    {
        Task<Request> GetForMethodAsync(string id, HttpMethods method);
    }
}
