using System;
using JSONMockifyAPI.Data.Models;
using JSONMockifyAPI.Services.Data.Contracts;
using Microsoft.AspNetCore.JsonPatch;
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
        
        [HttpGet()]
        public IActionResult Get()
        {
            return Ok(_jSONMockService.GetAll());
        }
        
        [HttpGet("{id}", Name = "GetMock")]
        public IActionResult Get(Guid id)
        {
            return Ok(_jSONMockService.GetById(id));
        }
        
        [HttpPost()]
        public IActionResult Post([FromBody] JSONMock newJSONMock)
        {
            var jsonMock = _jSONMockService.Add(newJSONMock);
            return CreatedAtRoute("GetMock", jsonMock);
        }

        // PUT api/jsonmock/5
        [HttpPut("{id}")]
        public IActionResult Put(Guid id, [FromBody] JSONMock updatedMock)
        {
            var jsonMock = _jSONMockService.Update(updatedMock);
            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult Patch(Guid id, [FromBody] JsonPatchDocument<JSONMock> updatedMock)
        {
            JSONMock model = new JSONMock();
            updatedMock.ApplyTo(model);
            var jsonMock = _jSONMockService.Update(model);
            return NoContent();
        }
        
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var jsonMock = _jSONMockService.Delete(id);
            return NoContent();
        }
    }
}
