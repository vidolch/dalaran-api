using System.Collections.Generic;
using JSONMockifyAPI.Data.Models;
using JSONMockifyAPI.Services.Data.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace JSONMockify.Web.APIClient.Controllers
{
    [Route("api/[controller]")]
    public class JSONMockController : Controller
    {
        private IJSONMockService _jSONMockService;

        public JSONMockController(IJSONMockService jSONMockService)
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
        public string Get(int id)
        {
            return "value";
        }

        // POST api/jsonmock
        [HttpPost]
        public JSONMock Post(JSONMock newJSONMock)
        {
            var jsonMock = _jSONMockService.Add(newJSONMock);
            return jsonMock;
        }

        // PUT api/jsonmock/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/jsonmock/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
