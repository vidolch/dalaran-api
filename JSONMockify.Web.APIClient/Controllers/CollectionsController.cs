// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace JSONMockify.Web.APIClient.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using JSONMockify.Web.APIClient.Dtos.Collection;
    using JSONMockifyAPI.Data.Models;
    using JSONMockifyAPI.Services.Data.Contracts;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.JsonPatch;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/collections")]
    [EnableCors("UI")]
    public class CollectionsController : Controller
    {
        private readonly ICollectionService colletionService;

        public CollectionsController(ICollectionService collectionService)
        {
            this.colletionService = collectionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var(collections, count) = await this.colletionService.GetAllAsync();

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

            return this.Ok(new CollectionDto(result));
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
                Name = newCollection.Name
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
                Name = updatedCollection.Name
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
                Name = model.Name
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
