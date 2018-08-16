// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace JSONMockify.Web.APIClient.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using JSONMockifyAPI.Data.Models;
    using JSONMockifyAPI.Services.Data.Contracts;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.JsonPatch;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/mocks")]
    [EnableCors("UI")]
    public class JSONMocksController : Controller
    {
        private readonly IJSONMockService jSONMockService;

        public JSONMocksController(IJSONMockService jSONMockService)
        {
            this.jSONMockService = jSONMockService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            IEnumerable<JSONMock> mocks = this.jSONMockService.GetAll().ToList();
            return this.Ok(mocks);
        }

        [HttpGet("{id}", Name = "GetMock")]
        public IActionResult Get(Guid id)
        {
            var result = this.jSONMockService.Get(id);
            if (result == null)
            {
                return this.NotFound();
            }

            return this.Ok(result);
        }

        [HttpPost]
        public IActionResult Post([FromBody] JSONMock newJSONMock)
        {
            if (newJSONMock == null)
            {
                return this.BadRequest();
            }

            var jsonMock = this.jSONMockService.Create(newJSONMock);
            return this.CreatedAtRoute("GetMock", new { id = jsonMock.ID }, jsonMock);
        }

        [HttpPut("{id}")]
        public IActionResult Put(Guid id, [FromBody] JSONMock updatedMock)
        {
            if (updatedMock == null)
            {
                return this.BadRequest();
            }

            if (!this.jSONMockService.RecordExists(id))
            {
                return this.NotFound();
            }

            updatedMock.ID = id;
            var jsonMock = this.jSONMockService.Update(updatedMock);
            return this.NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult Patch(Guid id, [FromBody] JsonPatchDocument<JSONMock> updatedMock)
        {
            if (updatedMock == null)
            {
                return this.BadRequest();
            }

            if (!this.jSONMockService.RecordExists(id))
            {
                return this.NotFound();
            }

            JSONMock model = new JSONMock();
            updatedMock.ApplyTo(model);
            model.ID = id;
            var jsonMock = this.jSONMockService.Update(model);
            return this.NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            if (!this.jSONMockService.RecordExists(id))
            {
                return this.NotFound();
            }

            this.jSONMockService.Delete(id);
            return this.NoContent();
        }
    }
}
