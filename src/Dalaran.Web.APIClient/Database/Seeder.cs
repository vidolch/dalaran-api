// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace Dalaran.Web.APIClient.Database
{
    using System.Threading.Tasks;
    using Dalaran.Data.Models;
    using Dalaran.Services.Data.Contracts;

    public class Seeder
    {
        private readonly ICollectionService collectionService;
        private readonly IResourceService resourceService;
        private readonly IRequestService requestService;

        public Seeder(
            ICollectionService collectionService,
            IResourceService resourceService,
            IRequestService requestService)
        {
            this.collectionService = collectionService;
            this.resourceService = resourceService;
            this.requestService = requestService;
        }

        public async Task Seed()
        {
            try
            {
                if (await this.collectionService.RecordExistsAsync("collection1"))
                {
                    return;
                }
            }
            catch (System.Exception ex)
            {

                throw;
            }

            var collection1 = new Collection
            {
                ID = "collection1",
                Name = "Collection 1"
            };
            await this.collectionService.AddOrUpdateAsync(collection1);

            var resource1 = new Resource
            {
                ID = "resource1",
                Name = "Resource 1",
                Path = "resource1",
                CollectionId = collection1.ID
            };
            await this.resourceService.AddOrUpdateAsync(resource1);

            var request1 = new Request
            {
                ID = "request1",
                Name = "Request 1",
                Template = "Template",
                HttpMethod = HttpMethods.GET,
                ResourceId = resource1.ID
            };
            await this.requestService.AddOrUpdateAsync(request1);
        }
    }
}
