// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace Dalaran.Web.APIClient.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using Dalaran.Data.Models;
    using Dalaran.Services.Data.Contracts;
    using Dalaran.Web.APIClient.Dtos;
    using Dalaran.Web.APIClient.Dtos.Request;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.JsonPatch;
    using Microsoft.AspNetCore.Mvc;

    [Route("api")]
    [Authorize]
    public class RequestsController : Controller
    {
        private readonly ICollectionService collectionService;
        private readonly IResourceService resourceService;
        private readonly IRequestService requestService;

        public RequestsController(
            ICollectionService collectionService,
            IResourceService resourceService,
            IRequestService requestService)
        {
            this.collectionService = collectionService;
            this.resourceService = resourceService;
            this.requestService = requestService;
        }

        [HttpGet("collections/{collectionId}/resources/{resourceId}/requests")]
        public async Task<IActionResult> Get(string collectionId, string resourceId)
        {
            if (!await this.collectionService.RecordExistsAsync(collectionId))
            {
                return this.NotFound($"Collection with id {collectionId} not found.");
            }

            var resource = await this.resourceService.GetAsync(resourceId);
            if (resource == null || resource.CollectionId != collectionId)
            {
                return this.NotFound($"Resource with id {resourceId} not found for collection with id {collectionId} not found.");
            }

            var (requests, count) = await this.requestService.GetAllAsync(x => x.ResourceId == resourceId);

            return this.Ok(new RequestListDto(1, count, requests.Select(c => new RequestDto(c))));
        }

        [HttpGet("collections/{collectionId}/resources/{resourceId}/requests/{id}", Name = "GetRequest")]
        public async Task<IActionResult> Get(string collectionId, string resourceId, string id)
        {
            if (!await this.collectionService.RecordExistsAsync(collectionId))
            {
                return this.NotFound($"Collection with id {collectionId} not found.");
            }

            var resource = await this.resourceService.GetAsync(resourceId);
            if (resource == null || resource.CollectionId != collectionId)
            {
                return this.NotFound($"Resource with id {resourceId} not found for collection with id {collectionId} not found.");
            }

            var result = await this.requestService.GetAsync(id);
            if (result == null || result.ResourceId != resourceId)
            {
                return this.NotFound($"Request with id {id} not found for Resource with id {resourceId} and collection with id {collectionId} not found.");
            }

            return this.Ok(new RequestDto(result));
        }

        [HttpPost("collections/{collectionId}/resources/{resourceId}/requests")]
        public async Task<IActionResult> Post(string collectionId, string resourceId, [FromBody] RequestUpdateDto newRequest)
        {
            if (newRequest == null)
            {
                return this.BadRequest();
            }

            if (!await this.collectionService.RecordExistsAsync(collectionId))
            {
                return this.NotFound($"Collection with id {collectionId} not found.");
            }

            var resource = await this.resourceService.GetAsync(resourceId);
            if (resource == null || resource.CollectionId != collectionId)
            {
                return this.NotFound($"Resource with id {resourceId} not found for collection with id {collectionId} not found.");
            }

            Request requestToSave = newRequest.ToModel(null, resourceId);

            await this.requestService.AddOrUpdateAsync(requestToSave);
            return this.CreatedAtRoute("GetRequest", new { id = requestToSave.ID }, new RequestDto(requestToSave));
        }

        [HttpPut("collections/{collectionId}/resources/{resourceId}/requests/{id}")]
        public async Task<IActionResult> Put(string collectionId, string resourceId, string id, [FromBody] RequestUpdateDto updatedRequest)
        {
            if (updatedRequest == null)
            {
                return this.BadRequest();
            }

            if (!await this.collectionService.RecordExistsAsync(collectionId))
            {
                return this.NotFound($"Collection with id {collectionId} not found.");
            }

            var resource = await this.resourceService.GetAsync(resourceId);
            if (resource == null || resource.CollectionId != collectionId)
            {
                return this.NotFound($"Resource with id {resourceId} not found for collection with id {collectionId} not found.");
            }

            var result = await this.requestService.GetAsync(id);
            if (result == null || result.ResourceId != resourceId)
            {
                return this.NotFound($"Request with id {id} not found for Resource with id {resourceId} and collection with id {collectionId} not found.");
            }

            Request requestToSave = updatedRequest.ToModel(id, resourceId);

            await this.requestService.AddOrUpdateAsync(requestToSave);
            return this.NoContent();
        }

        [HttpPatch("collections/{collectionId}/resources/{resourceId}/requests/{id}")]
        public async Task<IActionResult> Patch(string collectionId, string resourceId, string id, [FromBody] JsonPatchDocument<RequestUpdateDto> updatedRequest)
        {
            if (updatedRequest == null)
            {
                return this.BadRequest();
            }

            if (!await this.collectionService.RecordExistsAsync(collectionId))
            {
                return this.NotFound($"Collection with id {collectionId} not found.");
            }

            var resource = await this.resourceService.GetAsync(resourceId);
            if (resource == null || resource.CollectionId != collectionId)
            {
                return this.NotFound($"Resource with id {resourceId} not found for collection with id {collectionId} not found.");
            }

            var result = await this.requestService.GetAsync(id);
            if (result == null || result.ResourceId != resourceId)
            {
                return this.NotFound($"Request with id {id} not found for Resource with id {resourceId} and collection with id {collectionId} not found.");
            }

            RequestUpdateDto model = new RequestUpdateDto();
            updatedRequest.ApplyTo(model);

            Request requestToSave = model.ToModel(id, resourceId);

            await this.requestService.AddOrUpdateAsync(requestToSave);
            return this.NoContent();
        }

        [HttpDelete("collections/{collectionId}/resources/{resourceId}/requests/{id}")]
        public async Task<IActionResult> Delete(string collectionId, string resourceId, string id)
        {
            if (!await this.collectionService.RecordExistsAsync(collectionId))
            {
                return this.NotFound($"Collection with id {collectionId} not found.");
            }

            var resource = await this.resourceService.GetAsync(resourceId);
            if (resource == null || resource.CollectionId != collectionId)
            {
                return this.NotFound($"Resource with id {resourceId} not found for collection with id {collectionId} not found.");
            }

            var result = await this.requestService.GetAsync(id);
            if (result == null || result.ResourceId != resourceId)
            {
                return this.NotFound($"Request with id {id} not found for Resource with id {resourceId} and collection with id {collectionId} not found.");
            }

            await this.requestService.DeleteAsync(id);
            return this.NoContent();
        }
    }
}
