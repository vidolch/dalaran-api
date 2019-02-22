// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace Dalaran.Web.APIClient.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Dalaran.Core.Domain;
    using Dalaran.Data.Models;
    using Dalaran.Data.Repositories.Interfaces;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/fake")]
    [EnableCors("UI")]
    public class FakeController : Controller
    {
        private readonly IRequestRepository requestRepository;
        private readonly IApiGenerator apiGenerator;
        private readonly ICollectionRepository collectionRepository;
        private readonly IResourceRepository resourceRepository;

        public FakeController(
            ICollectionRepository collectionRepository,
            IResourceRepository resourceRepository,
            IRequestRepository requestRepository,
            IApiGenerator apiGenerator)
        {
            this.collectionRepository = collectionRepository;
            this.resourceRepository = resourceRepository;
            this.requestRepository = requestRepository;
            this.apiGenerator = apiGenerator;
        }

        [HttpGet("{id}", Name = "GetFake")]
        public async Task<IActionResult> GetAsync(string id)
        {
            var collection = await this.collectionRepository.GetAsync(id);
            collection.Resources = (await this.resourceRepository.GetAllAsync(x => x.CollectionId == collection.ID)).Item1;

            collection.Resources = collection.Resources.Select(r =>
            {
                r.Requests = this.requestRepository.GetAllAsync(x => x.ResourceId == r.ID).GetAwaiter().GetResult().Item1;
                return r;
            });

            var api = this.apiGenerator.GenerateApi(new ApiConfiguration {
                Collections = new List<Collection> { collection }
            });

            return this.File(api.Archive, "application/zip");
        }
    }
}
