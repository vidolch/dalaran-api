using System;
using System.Collections.Generic;
using JSONMockifyAPI.Data.Models;
using JSONMockifyAPI.Services.Data.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace JSONMockify.Web.APIClient.Controllers
{
    [Route("api/mocks")]
    public class JSONMocksController : Controller
    {
        private IJSONMockService _jSONMockService;

        public JSONMocksController(IJSONMockService jSONMockService)
        {
            _jSONMockService = jSONMockService;
        }
        // GET api/jsonmock
        [HttpGet]
        public IEnumerable<JSONMock> Get()
        {
            return _jSONMockService.GetAll();
        }

        // GET api/jsonmock/5
        [HttpGet("{id}")]
        public string Get(Guid id)
        {
            return "value";
        }

        // POST api/jsonmock
        [HttpPost]
        public JSONMock Post([FromBody] JSONMock newJSONMock)
        {
            var jsonMock = _jSONMockService.Add(newJSONMock);
            return jsonMock;
        }

        // PUT api/jsonmock/5
        [HttpPut("{id}")]
        public void Put(Guid id, [FromBody]string value)
        {
        }

        // DELETE api/jsonmock/5
        [HttpDelete("{id}")]
        public void Delete(Guid id)
        {
        }
    }
}
