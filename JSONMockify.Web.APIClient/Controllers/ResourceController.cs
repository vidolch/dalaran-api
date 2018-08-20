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

    [Route("api/resources")]
    [EnableCors("UI")]
    public class ResourceController : Controller
    {
        private readonly IResourceService resourceService;

        public ResourceController(IResourceService resourceService)
        {
            this.resourceService = resourceService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            IEnumerable<Resource> resources = this.resourceService.GetAllAsync().GetAwaiter().GetResult().Item1;
            return this.Ok(resources);
        }

        [HttpGet("{id}", Name = "GetResource")]
        public IActionResult Get(string id)
        {
            var result = this.resourceService.GetAsync(id).GetAwaiter().GetResult();
            if (result == null)
            {
                return this.NotFound();
            }

            return this.Ok(result);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Resource newResource)
        {
            if (newResource == null)
            {
                return this.BadRequest();
            }

            this.resourceService.AddOrUpdateAsync(newResource);
            return this.CreatedAtRoute("GetResource", new { id = newResource.ID }, newResource);
        }

        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] Resource updatedResource)
        {
            if (updatedResource == null)
            {
                return this.BadRequest();
            }

            if (!this.resourceService.RecordExistsAsync(id).GetAwaiter().GetResult())
            {
                return this.NotFound();
            }

            updatedResource.ID = id;
            this.resourceService.AddOrUpdateAsync(updatedResource);
            return this.NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult Patch(string id, [FromBody] JsonPatchDocument<Resource> updatedResource)
        {
            if (updatedResource == null)
            {
                return this.BadRequest();
            }

            if (!this.resourceService.RecordExistsAsync(id).GetAwaiter().GetResult())
            {
                return this.NotFound();
            }

            Resource model = new Resource();
            updatedResource.ApplyTo(model);
            model.ID = id;
            this.resourceService.AddOrUpdateAsync(model);
            return this.NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            if (!this.resourceService.RecordExistsAsync(id).GetAwaiter().GetResult())
            {
                return this.NotFound();
            }

            this.resourceService.DeleteAsync(id);
            return this.NoContent();
        }
    }
}
