using System;
using System.Collections.Generic;
using System.Linq;
using JSONMockifyAPI.Data.Models;
using JSONMockifyAPI.Services.Data.Contracts;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace JSONMockify.Web.APIClient.Controllers
{
    [Route("api/mocks")]
    [EnableCors("UI")]
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
            IEnumerable<JSONMock> mocks = _jSONMockService.GetAll().ToList();
            return Ok(mocks);
        }
        
        [HttpGet("{id}", Name = "GetMock")]
        public IActionResult Get(Guid id)
        {
            var result = _jSONMockService.Get(id);
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

            var jsonMock = _jSONMockService.Create(newJSONMock);
            return CreatedAtRoute("GetMock", new { id = jsonMock.ID }, jsonMock);
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
            updatedMock.ID = id;
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
            model.ID = id;
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

            _jSONMockService.Delete(id);
            return NoContent();
        }
    }
}
