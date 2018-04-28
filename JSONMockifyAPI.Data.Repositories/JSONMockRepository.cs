using JSONMockifyAPI.Data.Models;
using JSONMockifyAPI.Data.Repositories.Databases;
using JSONMockifyAPI.Data.Repositories.Interfaces;

namespace JSONMockifyAPI.Data.Repositories
{
    public class JSONMockRepository : Repository<JSONMock>, IJSONMockRepository
    {
        public JSONMockRepository(IDBRepository<JSONMock> dbInstance) : base(dbInstance)
        {
        }
    }
}
