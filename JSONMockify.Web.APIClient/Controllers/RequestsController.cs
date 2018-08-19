// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace JSONMockify.Web.APIClient.Controllers
{
    using System.Collections.Generic;
    using JSONMockifyAPI.Data.Models;
    using JSONMockifyAPI.Services.Data.Contracts;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.JsonPatch;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/requests")]
    [EnableCors("UI")]
    public class RequestsController : Controller
    {
        private readonly IRequestService requestService;

        public RequestsController(IRequestService requestService)
        {
            this.requestService = requestService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            IEnumerable<Request> requests = this.requestService.GetAllAsync().GetAwaiter().GetResult().Item1;
            return this.Ok(requests);
        }

        [HttpGet("{id}", Name = "GetRequest")]
        public IActionResult Get(string id)
        {
            var result = this.requestService.GetAsync(id).GetAwaiter().GetResult();
            if (result == null)
            {
                return this.NotFound();
            }

            return this.Ok(result);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Request newRequest)
        {
            if (newRequest == null)
            {
                return this.BadRequest();
            }

            this.requestService.AddOrUpdateAsync(newRequest);
            return this.CreatedAtRoute("GetRequest", new { id = newRequest.ID }, newRequest);
        }

        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] Request updatedRequest)
        {
            if (updatedRequest == null)
            {
                return this.BadRequest();
            }

            if (!this.requestService.RecordExistsAsync(id).GetAwaiter().GetResult())
            {
                return this.NotFound();
            }

            updatedRequest.ID = id;
            this.requestService.AddOrUpdateAsync(updatedRequest);
            return this.NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult Patch(string id, [FromBody] JsonPatchDocument<Request> updatedRequest)
        {
            if (updatedRequest == null)
            {
                return this.BadRequest();
            }

            if (!this.requestService.RecordExistsAsync(id).GetAwaiter().GetResult())
            {
                return this.NotFound();
            }

            Request model = new Request();
            updatedRequest.ApplyTo(model);
            model.ID = id;
            this.requestService.AddOrUpdateAsync(model);
            return this.NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            if (!this.requestService.RecordExistsAsync(id).GetAwaiter().GetResult())
            {
                return this.NotFound();
            }

            this.requestService.DeleteAsync(id);
            return this.NoContent();
        }
    }
}
