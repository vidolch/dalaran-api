// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace Dalaran.Web.APIClient.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using Dalaran.Web.APIClient.Dtos.Resource;
    using Dalaran.Data.Models;
    using Dalaran.Services.Data.Contracts;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.JsonPatch;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/resources")]
    [EnableCors("UI")]
    public class ResourcesController : Controller
    {
        private readonly IResourceService resourceService;

        public ResourcesController(IResourceService resourceService)
        {
            this.resourceService = resourceService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var (resources, count) = await this.resourceService.GetAllAsync();
            return this.Ok(new ResourceListDto(1, count, resources.Select(r => new ResourceDto(r))));
        }

        [HttpGet("{id}", Name = "GetResource")]
        public async Task<IActionResult> GetAsync(string id)
        {
            var result = await this.resourceService.GetAsync(id);
            if (result == null)
            {
                return this.NotFound();
            }

            return this.Ok(new ResourceDto(result));
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] ResourceUpdateDto newResource)
        {
            if (newResource == null)
            {
                return this.BadRequest();
            }

            var resourceToSave = new Resource
            {
                Name = newResource.Name,
                Path = newResource.Path
            };

            await this.resourceService.AddOrUpdateAsync(resourceToSave);
            return this.CreatedAtRoute("GetResource", new { id = resourceToSave.ID }, new ResourceDto(resourceToSave));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(string id, [FromBody] ResourceUpdateDto updatedResource)
        {
            if (updatedResource == null)
            {
                return this.BadRequest();
            }

            if (!await this.resourceService.RecordExistsAsync(id))
            {
                return this.NotFound();
            }

            var resourceToSave = new Resource
            {
                ID = id,
                Name = updatedResource.Name,
                Path = updatedResource.Path
            };

            updatedResource.ID = id;
            await this.resourceService.AddOrUpdateAsync(resourceToSave);
            return this.NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchAsync(string id, [FromBody] JsonPatchDocument<ResourceUpdateDto> updatedResource)
        {
            if (updatedResource == null)
            {
                return this.BadRequest();
            }

            if (!await this.resourceService.RecordExistsAsync(id))
            {
                return this.NotFound();
            }

            ResourceUpdateDto model = new ResourceUpdateDto();
            updatedResource.ApplyTo(model);
            model.ID = id;

            Resource resourceToSave = new Resource
            {
                ID = id,
                Path = model.Path,
                Name = model.Name
            };
            await this.resourceService.AddOrUpdateAsync(resourceToSave);
            return this.NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            if (!await this.resourceService.RecordExistsAsync(id))
            {
                return this.NotFound();
            }

            await this.resourceService.DeleteAsync(id);
            return this.NoContent();
        }
    }
}
