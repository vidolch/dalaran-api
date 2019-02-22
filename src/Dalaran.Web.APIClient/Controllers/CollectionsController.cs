// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace Dalaran.Web.APIClient.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using Dalaran.Data.Models;
    using Dalaran.Services.Data.Contracts;
    using Dalaran.Web.APIClient.Dtos.Collection;
    using Dalaran.Web.APIClient.Dtos.Resource;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.JsonPatch;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/collections")]
    [Authorize]
    public class CollectionsController : Controller
    {
        private readonly ICollectionService colletionService;
        private readonly IResourceService resourceService;

        public CollectionsController(
            ICollectionService collectionService,
            IResourceService resourceService)
        {
            this.colletionService = collectionService;
            this.resourceService = resourceService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var (collections, count) = await this.colletionService.GetAllAsync();

            return this.Ok(new CollectionListDto(1, count, collections.Select(c => new CollectionDto(c))));
        }

        [HttpGet("{id}", Name = "GetCollection")]
        public async Task<IActionResult> GetAsync(string id)
        {
            var result = await this.colletionService.GetAsync(id);
            if (result == null)
            {
                return this.NotFound();
            }

            var collectionDto = new CollectionDto(result);
            var resources = await this.resourceService.GetAllAsync(x => x.CollectionId == collectionDto.ID);
            collectionDto.Resources = new ResourceListDto(1, resources.Item2, resources.Item1.Select(t => new ResourceDto(t)));

            return this.Ok(collectionDto);
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] CollectionUpdateDto newCollection)
        {
            if (newCollection == null)
            {
                return this.BadRequest();
            }

            if (!this.ModelState.IsValid)
            {
                return new BadRequestObjectResult(this.ModelState);
            }

            var collectionToSave = new Collection
            {
                Name = newCollection.Name,
            };

            await this.colletionService.AddOrUpdateAsync(collectionToSave);

            return this.CreatedAtRoute("GetCollection", new { id = collectionToSave.ID }, new CollectionDto(collectionToSave));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(string id, [FromBody] CollectionUpdateDto updatedCollection)
        {
            if (updatedCollection == null)
            {
                return this.BadRequest();
            }

            if (!this.ModelState.IsValid)
            {
                return new BadRequestObjectResult(this.ModelState);
            }

            if (!await this.colletionService.RecordExistsAsync(id))
            {
                return this.NotFound();
            }

            var collectionToSave = new Collection
            {
                ID = id,
                Name = updatedCollection.Name,
            };

            await this.colletionService.AddOrUpdateAsync(collectionToSave);
            return this.NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchAsync(string id, [FromBody] JsonPatchDocument<CollectionUpdateDto> updatedCollection)
        {
            if (updatedCollection == null)
            {
                return this.BadRequest();
            }

            if (!await this.colletionService.RecordExistsAsync(id))
            {
                return this.NotFound();
            }

            CollectionUpdateDto model = new CollectionUpdateDto();
            updatedCollection.ApplyTo(model);

            var collectionToSave = new Collection
            {
                ID = id,
                Name = model.Name,
            };
            await this.colletionService.AddOrUpdateAsync(collectionToSave);
            return this.NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            if (!await this.colletionService.RecordExistsAsync(id))
            {
                return this.NotFound();
            }

            await this.colletionService.DeleteAsync(id);
            return this.NoContent();
        }
    }
}
