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
            var result = _jSONMockService.GetById(id);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
        
        [HttpPost()]
        public IActionResult Post([FromBody] JSONMock newJSONMock)
        {
            if (newJSONMock == null)
            {
                return BadRequest();
            }

            var jsonMock = _jSONMockService.Add(newJSONMock);
            return CreatedAtRoute("GetMock", jsonMock);
        }
        
        [HttpPut("{id}")]
        public IActionResult Put(Guid id, [FromBody] JSONMock updatedMock)
        {
            if (updatedMock == null)
            {
                return BadRequest();
            }

            if (!_jSONMockService.RecordExists(id))
            {
                return NotFound();
            }

            var jsonMock = _jSONMockService.Update(updatedMock);
            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult Patch(Guid id, [FromBody] JsonPatchDocument<JSONMock> updatedMock)
        {
            if (updatedMock == null)
            {
                return BadRequest();
            }

            if (!_jSONMockService.RecordExists(id))
            {
                return NotFound();
            }

            JSONMock model = new JSONMock();
            updatedMock.ApplyTo(model);
            var jsonMock = _jSONMockService.Update(model);
            return NoContent();
        }
        
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            if (!_jSONMockService.RecordExists(id))
            {
                return NotFound();
            }

            var jsonMock = _jSONMockService.Delete(id);
            return NoContent();
        }
    }
}
