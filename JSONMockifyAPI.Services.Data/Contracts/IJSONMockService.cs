using JSONMockifyAPI.Data.Models;
using System;
using System.Collections.Generic;

namespace JSONMockifyAPI.Services.Data.Contracts
{
    public interface IJSONMockService
    {
        IEnumerable<JSONMock> GetAll();
        JSONMock Add(JSONMock jsonMock);
        JSONMock GetById(Guid id);
        bool Delete(Guid id);
        bool Update(JSONMock model);
    }
}
