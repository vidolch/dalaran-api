// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace Dalaran.Web.APIClient.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Dalaran.Data.Models;
    using Dalaran.Services.Data.Contracts;
    using Dalaran.Web.APIClient.Dtos.Request;
    using Dalaran.Web.APIClient.Dtos.Resource;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.JsonPatch;
    using Microsoft.AspNetCore.Mvc;

    [Route("api")]
    [EnableCors("UI")]
    public class ResourcesController : Controller
    {
        private readonly IResourceService resourceService;
        private readonly ICollectionService collectionService;
        private readonly IRequestService requestService;

        public ResourcesController(
            IResourceService resourceService,
            ICollectionService collectionService,
            IRequestService requestService)
        {
            this.resourceService = resourceService;
            this.collectionService = collectionService;
            this.requestService = requestService;
        }

        [HttpGet("collections/{collectionId}/resources")]
        public async Task<IActionResult> GetAsync(string collectionId)
        {
            if (!await this.collectionService.RecordExistsAsync(collectionId))
            {
                return this.NotFound($"Collection with id {collectionId} not found.");
            }

            var (resources, count) = await this.resourceService.GetAllAsync(x => x.CollectionId == collectionId);
            return this.Ok(new ResourceListDto(1, count, resources.Select(r => new ResourceDto(r))));
        }

        [HttpGet("collections/{collectionId}/resources/{id}", Name = "GetResource")]
        public async Task<IActionResult> GetAsync(string collectionId, string id)
        {
            if (!await this.collectionService.RecordExistsAsync(collectionId))
            {
                return this.NotFound($"Collection with id {collectionId} not found.");
            }

            var result = await this.resourceService.GetAsync(id);
            if (result == null || result.CollectionId != collectionId)
            {
                return this.NotFound($"Resource with id {id} not found for collection with id {collectionId}.");
            }

            var requests = await this.requestService.GetAllAsync(x => x.ResourceId == id);
            var resultDto = new ResourceDto(result);
            resultDto.Requests = new RequestListDto(1, requests.Item2, requests.Item1.Select(x => new RequestDto(x)));

            return this.Ok(resultDto);
        }

        [HttpPost("collections/{collectionId}/resources")]
        public async Task<IActionResult> PostAsync(string collectionId, [FromBody] ResourceUpdateDto newResource)
        {
            if (newResource == null)
            {
                return this.BadRequest();
            }

            var collection = await this.collectionService.GetAsync(collectionId);

            if (collection == null)
            {
                return this.NotFound($"Collection with id {collectionId} not found.");
            }

            var resourceToSave = new Resource
            {
                Name = newResource.Name,
                Path = newResource.Path,
                CollectionId = collectionId,
            };

            await this.resourceService.AddOrUpdateAsync(resourceToSave);
            return this.CreatedAtRoute("GetResource", new { collectionId, id = resourceToSave.ID }, new ResourceDto(resourceToSave));
        }

        [HttpPut("collections/{collectionId}/resources/{id}")]
        public async Task<IActionResult> PutAsync(string collectionId, string id, [FromBody] ResourceUpdateDto updatedResource)
        {
            if (updatedResource == null)
            {
                return this.BadRequest();
            }

            if (!await this.collectionService.RecordExistsAsync(collectionId))
            {
                return this.NotFound($"Collection with id {collectionId} not found.");
            }

            var resource = await this.resourceService.GetAsync(id);
            if (resource == null && resource.CollectionId == collectionId)
            {
                return this.NotFound($"Resource with id {id} not found for collection with id {collectionId}.");
            }

            var resourceToSave = new Resource
            {
                ID = id,
                Name = updatedResource.Name,
                CollectionId = updatedResource.CollectionId,
                Path = updatedResource.Path,
            };

            updatedResource.ID = id;
            await this.resourceService.AddOrUpdateAsync(resourceToSave);
            return this.NoContent();
        }

        [HttpPatch("collections/{collectionId}/resources/{id}")]
        public async Task<IActionResult> PatchAsync(string collectionId, string id, [FromBody] JsonPatchDocument<ResourceUpdateDto> updatedResource)
        {
            if (updatedResource == null)
            {
                return this.BadRequest();
            }

            if (!await this.collectionService.RecordExistsAsync(collectionId))
            {
                return this.NotFound($"Collection with id {collectionId} not found.");
            }

            var resource = await this.resourceService.GetAsync(id);
            if (resource == null && resource.CollectionId == collectionId)
            {
                return this.NotFound($"Resource with id {id} not found for collection with id {collectionId}.");
            }

            ResourceUpdateDto model = new ResourceUpdateDto();
            updatedResource.ApplyTo(model);
            model.ID = id;

            Resource resourceToSave = new Resource
            {
                ID = id,
                Path = model.Path,
                CollectionId = model.CollectionId,
                Name = model.Name,
            };
            await this.resourceService.AddOrUpdateAsync(resourceToSave);
            return this.NoContent();
        }

        [HttpDelete("collections/{collectionId}/resources/{id}")]
        public async Task<IActionResult> DeleteAsync(string collectionId, string id)
        {
            if (!await this.collectionService.RecordExistsAsync(collectionId))
            {
                return this.NotFound($"Collection with id {collectionId} not found.");
            }

            var resource = await this.resourceService.GetAsync(id);
            if (resource == null || resource.CollectionId != collectionId)
            {
                return this.NotFound($"Resource with id {id} not found for collection with id {collectionId}.");
            }

            await this.resourceService.DeleteAsync(id);
            return this.NoContent();
        }
    }
}
