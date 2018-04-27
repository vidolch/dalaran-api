using JSONMockifyAPI.Data.Models;
using JSONMockifyAPI.Services.Data.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public JSONMock GetById(Guid id)
        {
            return _jsonMocks.FirstOrDefault(j => j.ID == id);
        }

        public bool Delete(Guid id)
        {
            _jsonMocks.RemoveAll(j => j.ID == id);
            return true;
        }

        public bool Update(JSONMock model)
        {
            var modelToUpdate =_jsonMocks.FirstOrDefault(j => j.ID == model.ID);
            modelToUpdate = model;
            return true;
        }

        public bool RecordExists(Guid id)
        {
            return _jsonMocks.Any(j => j.ID == id);
        }
    }
}
