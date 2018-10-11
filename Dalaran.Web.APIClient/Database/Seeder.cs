using Dalaran.Data.Models;
using Dalaran.Services.Data.Contracts;

namespace Dalaran.Web.APIClient.Database
{
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

        public void Seed()
        {
            var collection1 = await this.collectionService.AddOrUpdateAsync(new Collection
            {
                ID = "collection1",
                Name = "Collection 1"
            });

            var resource1 = await this.resourceService.AddOrUpdateAsync(new Resource
            {
                ID = "collec",
                Name = "Resource 1",
                Path = "resource1",
                CollectionId = collection1.ID
            });

            var request1 = this.requestService.AddOrUpdateAsync(new Request
            {

            });
        }
    }
}
