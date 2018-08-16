// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace JSONMockify.Web.APIClient.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
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
            IEnumerable<JSONMock> mocks = this.jSONMockService.GetAllAsync().GetAwaiter().GetResult().Item1;
            return this.Ok(mocks);
        }

        [HttpGet("{id}", Name = "GetMock")]
        public IActionResult Get(string id)
        {
            var result = this.jSONMockService.GetAsync(id).GetAwaiter().GetResult();
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
            newJSONMock.ID = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);

            this.jSONMockService.AddOrUpdateAsync(newJSONMock.ID, newJSONMock);
            return this.CreatedAtRoute("GetMock", new { id = newJSONMock.ID }, newJSONMock);
        }

        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] JSONMock updatedMock)
        {
            if (updatedMock == null)
            {
                return this.BadRequest();
            }

            if (!this.jSONMockService.RecordExistsAsync(id).GetAwaiter().GetResult())
            {
                return this.NotFound();
            }

            updatedMock.ID = id;
            this.jSONMockService.AddOrUpdateAsync(updatedMock.ID, updatedMock);
            return this.NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult Patch(string id, [FromBody] JsonPatchDocument<JSONMock> updatedMock)
        {
            if (updatedMock == null)
            {
                return this.BadRequest();
            }

            if (!this.jSONMockService.RecordExistsAsync(id).GetAwaiter().GetResult())
            {
                return this.NotFound();
            }

            JSONMock model = new JSONMock();
            updatedMock.ApplyTo(model);
            model.ID = id;
            this.jSONMockService.AddOrUpdateAsync(model.ID, model);
            return this.NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            if (!this.jSONMockService.RecordExistsAsync(id).GetAwaiter().GetResult())
            {
                return this.NotFound();
            }

            this.jSONMockService.DeleteAsync(id);
            return this.NoContent();
        }
    }
}
