// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace JSONMockify.Web.APIClient.Controllers
{
    using JSONMockifyAPI.Data.Models;
    using JSONMockifyAPI.Services.Data.Contracts;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.JsonPatch;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;

    [Route("api/collections")]
    [EnableCors("UI")]
    public class CollectionController : Controller
    {
        private readonly ICollectionService colletionService;

        public CollectionController(ICollectionService collectionService)
        {
            this.colletionService = collectionService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            IEnumerable<Collection> collections = this.colletionService.GetAllAsync().GetAwaiter().GetResult().Item1;
            return this.Ok(collections);
        }

        [HttpGet("{id}", Name = "GetCollection")]
        public IActionResult Get(string id)
        {
            var result = this.colletionService.GetAsync(id).GetAwaiter().GetResult();
            if (result == null)
            {
                return this.NotFound();
            }

            return this.Ok(result);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Collection newCollection)
        {
            if (newCollection == null)
            {
                return this.BadRequest();
            }

            this.colletionService.AddOrUpdateAsync(newCollection);
            return this.CreatedAtRoute("GetCollection", new { id = newCollection.ID }, newCollection);
        }

        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] Collection updatedCollection)
        {
            if (updatedCollection == null)
            {
                return this.BadRequest();
            }

            if (!this.colletionService.RecordExistsAsync(id).GetAwaiter().GetResult())
            {
                return this.NotFound();
            }

            updatedCollection.ID = id;
            this.colletionService.AddOrUpdateAsync(updatedCollection);
            return this.NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult Patch(string id, [FromBody] JsonPatchDocument<Collection> updatedCollection)
        {
            if (updatedCollection == null)
            {
                return this.BadRequest();
            }

            if (!this.colletionService.RecordExistsAsync(id).GetAwaiter().GetResult())
            {
                return this.NotFound();
            }

            Collection model = new Collection();
            updatedCollection.ApplyTo(model);
            model.ID = id;
            this.colletionService.AddOrUpdateAsync(model);
            return this.NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            if (!this.colletionService.RecordExistsAsync(id).GetAwaiter().GetResult())
            {
                return this.NotFound();
            }

            this.colletionService.DeleteAsync(id);
            return this.NoContent();
        }
    }
}
