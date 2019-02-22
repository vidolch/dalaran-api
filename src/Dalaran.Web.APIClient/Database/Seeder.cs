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
            if (await this.collectionService.RecordExistsAsync("collection1"))
            {
                return;
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
                Name = "Test Get Request",
                Template = "Get Request",
                HttpMethod = HttpMethods.GET,
                ResourceId = resource1.ID
            };

            var request2 = new Request
            {
                ID = "request2",
                Name = "Test Head Request",
                Template = "Head Request",
                HttpMethod = HttpMethods.HEAD,
                ResourceId = resource1.ID
            };

            var request3 = new Request
            {
                ID = "request3",
                Name = "Test Post Request",
                Template = "Post Request",
                HttpMethod = HttpMethods.POST,
                ResourceId = resource1.ID
            };

            var request4 = new Request
            {
                ID = "request4",
                Name = "Test Put Request",
                Template = "Put Request",
                HttpMethod = HttpMethods.PUT,
                ResourceId = resource1.ID
            };

            var request5 = new Request
            {
                ID = "request5",
                Name = "Test Delete Request",
                Template = "Delete Request",
                HttpMethod = HttpMethods.DELETE,
                ResourceId = resource1.ID
            };

            var request6 = new Request
            {
                ID = "request6",
                Name = "Test Connect Request",
                Template = "Connect Request",
                HttpMethod = HttpMethods.CONNECT,
                ResourceId = resource1.ID
            };

            var request7 = new Request
            {
                ID = "request7",
                Name = "Test Options Request",
                Template = "Options Request",
                HttpMethod = HttpMethods.OPTIONS,
                ResourceId = resource1.ID
            };

            var request8 = new Request
            {
                ID = "request8",
                Name = "Test Trace Request",
                Template = "Trance Request",
                HttpMethod = HttpMethods.TRACE,
                ResourceId = resource1.ID
            };

            var request9 = new Request
            {
                ID = "request9",
                Name = "Test Patch Request",
                Template = "Patch request",
                HttpMethod = HttpMethods.PATCH,
                ResourceId = resource1.ID
            };

            await this.requestService.AddOrUpdateAsync(request1);
            await this.requestService.AddOrUpdateAsync(request2);
            await this.requestService.AddOrUpdateAsync(request3);
            await this.requestService.AddOrUpdateAsync(request4);
            await this.requestService.AddOrUpdateAsync(request5);
            await this.requestService.AddOrUpdateAsync(request6);
            await this.requestService.AddOrUpdateAsync(request7);
            await this.requestService.AddOrUpdateAsync(request8);
            await this.requestService.AddOrUpdateAsync(request9);
        }
    }
}
