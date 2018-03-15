using JSONMockifyAPI.Data.Models;
using JSONMockifyAPI.Services.Data.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSONMockifyAPI.Services.Data
{
    public class JSONMemoryService : IJSONMockService
    {
        private List<JSONMock> _jsonMocks;

        public JSONMemoryService()
        {
            _jsonMocks = new List<JSONMock>();
        }

        public JSONMock Add(JSONMock jsonMock)
        {
            _jsonMocks.Add(jsonMock);
            return jsonMock;
        }

        public IEnumerable<JSONMock> GetAll()
        {
            return _jsonMocks.OrderBy(j => j.ID);
        }
    }
}
