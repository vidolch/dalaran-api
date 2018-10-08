// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace Dalaran.Web.APIClient.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using Dalaran.Data.Models;
    using Dalaran.Services.Data.Contracts;
    using Dalaran.Web.APIClient.Dtos.Request;
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
        public async Task<IActionResult> Get()
        {
            var(requests, count) = await this.requestService.GetAllAsync();

            return this.Ok(new RequestListDto(1, count, requests.Select(c => new RequestDto(c))));
        }

        [HttpGet("{id}", Name = "GetRequest")]
        public async Task<IActionResult> Get(string id)
        {
            var result = await this.requestService.GetAsync(id);
            if (result == null)
            {
                return this.NotFound();
            }

            return this.Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RequestUpdateDto newRequest)
        {
            if (newRequest == null)
            {
                return this.BadRequest();
            }

            Request requestToSave = new Request
            {
                Name = newRequest.Name,
                Template = newRequest.Template,
                HttpMethod = newRequest.HttpMethod
            };
            await this.requestService.AddOrUpdateAsync(requestToSave);
            return this.CreatedAtRoute("GetRequest", new { id = requestToSave.ID }, new RequestDto(requestToSave));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] RequestUpdateDto updatedRequest)
        {
            if (updatedRequest == null)
            {
                return this.BadRequest();
            }

            if (!await this.requestService.RecordExistsAsync(id))
            {
                return this.NotFound();
            }

            var requestToSave = new Request
            {
                ID = id,
                Name = updatedRequest.Name,
                Template = updatedRequest.Template,
                HttpMethod = updatedRequest.HttpMethod
            };

            await this.requestService.AddOrUpdateAsync(requestToSave);
            return this.NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(string id, [FromBody] JsonPatchDocument<RequestUpdateDto> updatedRequest)
        {
            if (updatedRequest == null)
            {
                return this.BadRequest();
            }

            if (!await this.requestService.RecordExistsAsync(id))
            {
                return this.NotFound();
            }

            RequestUpdateDto model = new RequestUpdateDto();
            updatedRequest.ApplyTo(model);

            var requestToSave = new Request
            {
                ID = id,
                Name = model.Name,
                Template = model.Template,
                HttpMethod = model.HttpMethod
            };
            await this.requestService.AddOrUpdateAsync(requestToSave);
            return this.NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (!await this.requestService.RecordExistsAsync(id))
            {
                return this.NotFound();
            }

            await this.requestService.DeleteAsync(id);
            return this.NoContent();
        }
    }
}
