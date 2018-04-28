using JSONMockifyAPI.Data.Models;
using JSONMockifyAPI.Data.Repositories.Interfaces;
using JSONMockifyAPI.Services.Data.Contracts;

namespace JSONMockifyAPI.Services.Data
{
    public class JSONMockService : DataService<JSONMock>, IJSONMockService
    {
        public JSONMockService(IJSONMockRepository repository) : base(repository)
        {
        }
    }
}
