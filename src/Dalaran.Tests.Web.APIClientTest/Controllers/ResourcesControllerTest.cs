// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace Tests.Web.APIClientTest.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Dalaran.Data.Models;
    using Dalaran.Services.Data.Contracts;
    using Dalaran.Web.APIClient.Controllers;
    using Dalaran.Web.APIClient.Dtos.Resource;
    using Microsoft.AspNetCore.JsonPatch;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using Xunit;

    public class ResourcesControllerTest
    {
        [Fact]
        public async Task CanGetAllResourcesAsync()
        {
            const string collectionId = "collectionId";

            // Arrange
            var collectionService = new Mock<ICollectionService>();
            collectionService.Setup(repo => repo.RecordExistsAsync(collectionId)).Returns(Task.FromResult(true));

            var resourceService = new Mock<IResourceService>();
            resourceService.Setup(repo => repo.GetAllAsync(It.IsAny<Expression<Func<Resource, bool>>>(), It.IsAny<int>(), It.IsAny<int>())).Returns(this.GetTestResources());

            var requestService = new Mock<IRequestService>();

            var controller = new ResourcesController(resourceService.Object, collectionService.Object, requestService.Object);

            // Act
            var result = await controller.GetAsync(collectionId);

            // Assert
            var viewResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<ResourceListDto>(
                viewResult.Value);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task CanGetNoMocksAsync()
        {
            const string collectionId = "collectionId";

            // Arrange
            var collectionService = new Mock<ICollectionService>();
            collectionService.Setup(repo => repo.RecordExistsAsync(collectionId)).Returns(Task.FromResult(true));

            var resourceService = new Mock<IResourceService>();
            resourceService.Setup(repo => repo.GetAllAsync(null, 0, 20)).Returns(Task.FromResult((new List<Resource>().AsEnumerable(), 0L)));

            var requestService = new Mock<IRequestService>();
            requestService.Setup(repo => repo.GetAllAsync(null, 0, 20)).Returns(this.GetTestRequests());

            var controller = new ResourcesController(resourceService.Object, collectionService.Object, requestService.Object);

            // Act
            var result = await controller.GetAsync(collectionId);

            // Assert
            var viewResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<ResourceListDto>(
                viewResult.Value);
            Assert.Empty(model);
        }

        [Fact]
        public async Task CanGetMockByIdAsync()
        {
            const string collectionId = "collectionId";

            // Arrange
            var collectionService = new Mock<ICollectionService>();
            collectionService.Setup(repo => repo.RecordExistsAsync(collectionId)).Returns(Task.FromResult(true));

            string testId = "id1";
            DateTimeOffset testCreated = new DateTime(2017, 12, 2);
            string testTemplate = "Test Template";
            var resourceService = new Mock<IResourceService>();
            resourceService.Setup(repo => repo.GetAsync(testId)).Returns(Task.FromResult(new Resource
            {
                ID = testId,
                Name = testTemplate,
                CreatedTimestamp = testCreated,
                CollectionId = collectionId,
            }));

            var requestService = new Mock<IRequestService>();
            requestService.Setup(repo => repo.GetAllAsync(null, 0, 20)).Returns(this.GetTestRequests());

            var controller = new ResourcesController(resourceService.Object, collectionService.Object, requestService.Object);

            // Act
            var result = await controller.GetAsync(collectionId, testId);

            // Assert
            var viewResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<ResourceDto>(
                viewResult.Value);
            Assert.Equal(testId, model.ID);
            Assert.Equal(testTemplate, model.Name);
            Assert.Equal(testCreated, model.CreatedTimestamp);
        }

        [Fact]
        public async Task CanGetNoMockByIdAsync()
        {
            const string collectionId = "collectionId";

            // Arrange
            var collectionService = new Mock<ICollectionService>();
            collectionService.Setup(repo => repo.RecordExistsAsync(collectionId)).Returns(Task.FromResult(true));

            string testId = "id1";
            var resourceService = new Mock<IResourceService>();
            resourceService.Setup(repo => repo.GetAsync(testId)).Returns(Task.FromResult((Resource)null));

            var requestService = new Mock<IRequestService>();
            requestService.Setup(repo => repo.GetAllAsync(null, 0, 20)).Returns(this.GetTestRequests());

            var controller = new ResourcesController(resourceService.Object, collectionService.Object, requestService.Object);

            // Act
            var result = await controller.GetAsync(collectionId, testId);

            // Assert
            var viewResult = Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task CanCreateResourceAsync()
        {
            const string collectionId = "collectionId";

            // Arrange
            var collectionService = new Mock<ICollectionService>();
            collectionService.Setup(repo => repo.GetAsync(collectionId)).Returns(Task.FromResult(new Collection { ID = collectionId }));

            Resource testResource = new Resource
            {
                Name = "Test Template",
            };
            ResourceUpdateDto testResourceDto = new ResourceUpdateDto
            {
                Name = "Test Template",
            };
            var resourceService = new Mock<IResourceService>();
            resourceService.Setup(repo => repo.AddOrUpdateAsync(testResource)).Returns(Task.FromResult(testResource));

            var requestService = new Mock<IRequestService>();
            requestService.Setup(repo => repo.GetAllAsync(null, 0, 20)).Returns(this.GetTestRequests());

            var controller = new ResourcesController(resourceService.Object, collectionService.Object, requestService.Object);

            // Act
            var result = await controller.PostAsync(collectionId, testResourceDto);

            // Assert
            var viewResult = Assert.IsType<CreatedAtRouteResult>(result);
            var model = Assert.IsAssignableFrom<ResourceDto>(
                viewResult.Value);
            Assert.Equal(testResource.Name, model.Name);
        }

        [Fact]
        public async Task CannotCreateResourceWhenEmptyResourceAsync()
        {
            const string collectionId = "collectionId";

            // Arrange
            var collectionService = new Mock<ICollectionService>();
            collectionService.Setup(repo => repo.RecordExistsAsync(collectionId)).Returns(Task.FromResult(true));

            var resourceService = new Mock<IResourceService>();

            var requestService = new Mock<IRequestService>();
            requestService.Setup(repo => repo.GetAllAsync(null, 0, 20)).Returns(this.GetTestRequests());

            var controller = new ResourcesController(resourceService.Object, collectionService.Object, requestService.Object);

            // Act
            var result = await controller.PostAsync(collectionId, null);

            // Assert
            var viewResult = Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task CanPutResourceAsync()
        {
            const string collectionId = "collectionId";

            // Arrange
            var collectionService = new Mock<ICollectionService>();
            collectionService.Setup(repo => repo.RecordExistsAsync(collectionId)).Returns(Task.FromResult(true));

            ResourceUpdateDto testResourceDto = new ResourceUpdateDto
            {
                Name = "Test Template",
                CollectionId = collectionId,
            };
            Resource testResource = new Resource
            {
                Name = "Test Template",
                CollectionId = collectionId,
            };
            string testID = "id1";
            var resourceService = new Mock<IResourceService>();
            resourceService.Setup(repo => repo.AddOrUpdateAsync(testResource)).Returns(Task.FromResult(testResource));
            resourceService.Setup(repo => repo.GetAsync(testID)).Returns(Task.FromResult(testResource));

            var requestService = new Mock<IRequestService>();
            requestService.Setup(repo => repo.GetAllAsync(null, 0, 20)).Returns(this.GetTestRequests());

            var controller = new ResourcesController(resourceService.Object, collectionService.Object, requestService.Object);

            // Act
            var result = await controller.PutAsync(collectionId, testID, testResourceDto);

            // Assert
            var viewResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task CannotPutResourceWhenEmptyRequestAsync()
        {
            const string collectionId = "collectionId";

            // Arrange
            var collectionService = new Mock<ICollectionService>();
            collectionService.Setup(repo => repo.RecordExistsAsync(collectionId)).Returns(Task.FromResult(true));

            var resourceService = new Mock<IResourceService>();

            var requestService = new Mock<IRequestService>();
            requestService.Setup(repo => repo.GetAllAsync(null, 0, 20)).Returns(this.GetTestRequests());

            var controller = new ResourcesController(resourceService.Object, collectionService.Object, requestService.Object);

            // Act
            var result = await controller.PutAsync(collectionId, "id1", null);

            // Assert
            var viewResult = Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task CanPatchResourceAsync()
        {
            const string collectionId = "collectionId";

            // Arrange
            var collectionService = new Mock<ICollectionService>();
            collectionService.Setup(repo => repo.RecordExistsAsync(collectionId)).Returns(Task.FromResult(true));

            Resource testResource = new Resource
            {
                Name = "Test Template",
                CollectionId = collectionId,
            };
            string testID = "id1";
            var resourceService = new Mock<IResourceService>();
            resourceService.Setup(repo => repo.AddOrUpdateAsync(testResource)).Returns(Task.FromResult(testResource));
            resourceService.Setup(repo => repo.GetAsync(testID)).Returns(Task.FromResult(testResource));

            var requestService = new Mock<IRequestService>();
            requestService.Setup(repo => repo.GetAllAsync(null, 0, 20)).Returns(this.GetTestRequests());

            var controller = new ResourcesController(resourceService.Object, collectionService.Object, requestService.Object);

            // Act
            var result = await controller.PatchAsync(collectionId, testID, new JsonPatchDocument<ResourceUpdateDto>());

            // Assert
            var viewResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task CannotPatchResourceWhenEmptyRequestAsync()
        {
            const string collectionId = "collectionId";

            // Arrange
            var collectionService = new Mock<ICollectionService>();
            collectionService.Setup(repo => repo.RecordExistsAsync(collectionId)).Returns(Task.FromResult(true));

            var resourceService = new Mock<IResourceService>();

            var requestService = new Mock<IRequestService>();
            requestService.Setup(repo => repo.GetAllAsync(null, 0, 20)).Returns(this.GetTestRequests());

            var controller = new ResourcesController(resourceService.Object, collectionService.Object, requestService.Object);

            // Act
            var result = await controller.PatchAsync(collectionId, "id1", null);

            // Assert
            var viewResult = Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task CanDeleteResourceAsync()
        {
            const string collectionId = "collectionId";

            // Arrange
            var collectionService = new Mock<ICollectionService>();
            collectionService.Setup(repo => repo.RecordExistsAsync(collectionId)).Returns(Task.FromResult(true));

            Resource testResource = new Resource
            {
                Name = "Test Template",
                CollectionId = collectionId,
            };
            string testID = "id1";
            var resourceService = new Mock<IResourceService>();
            resourceService.Setup(repo => repo.GetAsync(testID)).Returns(Task.FromResult(testResource));

            var requestService = new Mock<IRequestService>();
            requestService.Setup(repo => repo.GetAllAsync(null, 0, 20)).Returns(this.GetTestRequests());

            var controller = new ResourcesController(resourceService.Object, collectionService.Object, requestService.Object);

            // Act
            var result = await controller.DeleteAsync(collectionId, "id1");

            // Assert
            var viewResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task CannotDeleteResourceWhenIDNotExistsAsync()
        {
            const string collectionId = "collectionId";

            // Arrange
            var collectionService = new Mock<ICollectionService>();
            collectionService.Setup(repo => repo.RecordExistsAsync(collectionId)).Returns(Task.FromResult(true));

            var resourceService = new Mock<IResourceService>();

            var requestService = new Mock<IRequestService>();
            requestService.Setup(repo => repo.GetAllAsync(null, 0, 20)).Returns(this.GetTestRequests());

            var controller = new ResourcesController(resourceService.Object, collectionService.Object, requestService.Object);

            // Act
            var result = await controller.DeleteAsync(collectionId, "id1");

            // Assert
            var viewResult = Assert.IsType<NotFoundObjectResult>(result);
        }

        private Task<(IEnumerable<Request>, long)> GetTestRequests()
        {
            var mocks = new List<Request>();
            mocks.Add(new Request()
            {
                CreatedTimestamp = new DateTime(2016, 7, 2),
                ID = "id1",
                Template = "Test One",
            });
            mocks.Add(new Request()
            {
                CreatedTimestamp = new DateTime(2016, 7, 3),
                ID = "id2",
                Template = "Test Two",
            });
            return Task.FromResult((mocks.AsEnumerable(), (long)mocks.Count));
        }

        private Task<(IEnumerable<Resource>, long)> GetTestResources()
        {
            var mocks = new List<Resource>();
            mocks.Add(new Resource()
            {
                ID = "id1",
                Name = "Test One",
            });
            mocks.Add(new Resource()
            {
                ID = "id2",
                Name = "Test Two",
            });
            return Task.FromResult((mocks.AsEnumerable(), (long)mocks.Count));
        }
    }
}
