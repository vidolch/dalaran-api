using JSONMockifyAPI.Data.Models;
using System.Collections.Generic;

namespace JSONMockifyAPI.Services.Data.Contracts
{
    public interface IJSONMockService
    {
        IEnumerable<JSONMock> GetAll();
        JSONMock Add(JSONMock jsonMock);
    }
}
