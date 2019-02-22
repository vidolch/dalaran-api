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
    using Dalaran.Web.APIClient.Dtos.Request;
    using Microsoft.AspNetCore.JsonPatch;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using Xunit;

    public class RequestsControllerTest
    {
        [Fact]
        public async Task CanGetAllMocks()
        {
            const string collectionId = "collectionId";
            const string resourceId = "resourceId";

            // Arrange
            var collectionService = new Mock<ICollectionService>();
            collectionService.Setup(repo => repo.RecordExistsAsync(collectionId)).Returns(Task.FromResult(true));

            var reosurceService = new Mock<IResourceService>();
            reosurceService.Setup(repo => repo.GetAsync(resourceId)).Returns(this.GetTestResource(collectionId));

            var requestService = new Mock<IRequestService>();
            requestService.Setup(repo => repo.GetAllAsync(It.IsAny<Expression<Func<Request, bool>>>(), It.IsAny<int>(), It.IsAny<int>())).Returns(this.GetTestRequests());

            var controller = new RequestsController(collectionService.Object, reosurceService.Object, requestService.Object);

            // Act
            var result = await controller.Get(collectionId, resourceId);

            // Assert
            var viewResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<RequestListDto>(
                viewResult.Value);
            Assert.Equal(2, model.Contents.Count);
        }

        [Fact]
        public async Task CanGetNoMocks()
        {
            const string collectionId = "collectionId";
            const string resourceId = "resourceId";

            // Arrange
            var collectionService = new Mock<ICollectionService>();
            collectionService.Setup(repo => repo.RecordExistsAsync(collectionId)).Returns(Task.FromResult(true));

            var reosurceService = new Mock<IResourceService>();
            reosurceService.Setup(repo => repo.GetAsync(resourceId)).Returns(this.GetTestResource(collectionId));

            var requestService = new Mock<IRequestService>();
            requestService.Setup(repo => repo.GetAllAsync(null, 0, 20)).Returns(Task.FromResult((new List<Request>().AsEnumerable(), 0L)));
            var controller = new RequestsController(collectionService.Object, reosurceService.Object, requestService.Object);

            // Act
            var result = await controller.Get(collectionId, resourceId);

            // Assert
            var viewResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<RequestListDto>(
                viewResult.Value);
            Assert.Empty(model.Contents);
        }

        [Fact]
        public async Task CanGetMockById()
        {
            const string collectionId = "collectionId";
            const string resourceId = "resourceId";

            // Arrange
            var collectionService = new Mock<ICollectionService>();
            collectionService.Setup(repo => repo.RecordExistsAsync(collectionId)).Returns(Task.FromResult(true));

            var reosurceService = new Mock<IResourceService>();
            reosurceService.Setup(repo => repo.GetAsync(resourceId)).Returns(this.GetTestResource(collectionId));

            string testId = "id1";
            DateTimeOffset testCreated = new DateTime(2017, 12, 2);
            string testTemplate = "Test Template";
            var requestService = new Mock<IRequestService>();
            requestService.Setup(repo => repo.GetAsync(testId)).Returns(Task.FromResult(new Request
            {
                ID = testId,
                CreatedTimestamp = testCreated,
                Template = testTemplate,
                ResourceId = resourceId,
            }));

            var controller = new RequestsController(collectionService.Object, reosurceService.Object, requestService.Object);

            // Act
            var result = await controller.Get(collectionId, resourceId, testId);

            // Assert
            var viewResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<RequestDto>(
                viewResult.Value);
            Assert.Equal(testId, model.ID);
            Assert.Equal(testTemplate, model.Template);
            Assert.Equal(testCreated, model.CreatedTimestamp);
        }

        [Fact]
        public async Task CanGetNoMockById()
        {
            const string collectionId = "collectionId";
            const string resourceId = "resourceId";

            // Arrange
            var collectionService = new Mock<ICollectionService>();
            collectionService.Setup(repo => repo.RecordExistsAsync(collectionId)).Returns(Task.FromResult(true));

            var reosurceService = new Mock<IResourceService>();
            reosurceService.Setup(repo => repo.GetAsync(resourceId)).Returns(this.GetTestResource(collectionId));

            string testId = "id1";
            var requestService = new Mock<IRequestService>();
            requestService.Setup(repo => repo.GetAsync(testId)).Returns(Task.FromResult((Request)null));

            var controller = new RequestsController(collectionService.Object, reosurceService.Object, requestService.Object);

            // Act
            var result = await controller.Get(collectionId, resourceId, testId);

            // Assert
            var viewResult = Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task CanCreateRequest()
        {
            const string collectionId = "collectionId";
            const string resourceId = "resourceId";

            // Arrange
            var collectionService = new Mock<ICollectionService>();
            collectionService.Setup(repo => repo.RecordExistsAsync(collectionId)).Returns(Task.FromResult(true));

            var reosurceService = new Mock<IResourceService>();
            reosurceService.Setup(repo => repo.GetAsync(resourceId)).Returns(this.GetTestResource(collectionId));

            Request testRequest = new Request
            {
                Template = "Test Template",
            };
            var requestService = new Mock<IRequestService>();
            requestService.Setup(repo => repo.AddOrUpdateAsync(testRequest)).Returns(Task.FromResult(testRequest));

            var controller = new RequestsController(collectionService.Object, reosurceService.Object, requestService.Object);

            RequestUpdateDto requestDto = new RequestUpdateDto
            {
                Name = "Test Name",
                Template = testRequest.Template,
            };

            // Act
            var result = await controller.Post(collectionId, resourceId, requestDto);

            // Assert
            var viewResult = Assert.IsType<CreatedAtRouteResult>(result);
            var model = Assert.IsAssignableFrom<RequestDto>(
                viewResult.Value);
            Assert.Equal(testRequest.Template, model.Template);
        }

        [Fact]
        public async Task CannotCreateRequestWhenEmptyRequest()
        {
            const string collectionId = "collectionId";
            const string resourceId = "resourceId";

            // Arrange
            var collectionService = new Mock<ICollectionService>();
            collectionService.Setup(repo => repo.RecordExistsAsync(collectionId)).Returns(Task.FromResult(true));

            var reosurceService = new Mock<IResourceService>();
            reosurceService.Setup(repo => repo.GetAsync(resourceId)).Returns(this.GetTestResource(collectionId));

            var requestService = new Mock<IRequestService>();

            var controller = new RequestsController(collectionService.Object, reosurceService.Object, requestService.Object);

            // Act
            var result = await controller.Post(collectionId, resourceId, null);

            // Assert
            var viewResult = Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task CanPutRequest()
        {
            const string collectionId = "collectionId";
            const string resourceId = "resourceId";

            // Arrange
            var collectionService = new Mock<ICollectionService>();
            collectionService.Setup(repo => repo.RecordExistsAsync(collectionId)).Returns(Task.FromResult(true));

            var reosurceService = new Mock<IResourceService>();
            reosurceService.Setup(repo => repo.GetAsync(resourceId)).Returns(this.GetTestResource(collectionId));

            Request testRequest = new Request
            {
                Template = "Test Template",
                ResourceId = resourceId,
            };
            string testID = "id1";
            var requestService = new Mock<IRequestService>();
            requestService.Setup(repo => repo.AddOrUpdateAsync(testRequest)).Returns(Task.FromResult(testRequest));
            requestService.Setup(repo => repo.GetAsync(testID)).Returns(Task.FromResult(testRequest));

            var controller = new RequestsController(collectionService.Object, reosurceService.Object, requestService.Object);

            RequestUpdateDto requestDto = new RequestUpdateDto
            {
                Name = "Test Name",
                Template = testRequest.Template,
            };

            // Act
            var result = await controller.Put(collectionId, resourceId, testID, requestDto);

            // Assert
            var viewResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task CannotPutRequestWhenEmptyRequest()
        {
            const string collectionId = "collectionId";
            const string resourceId = "resourceId";

            // Arrange
            var collectionService = new Mock<ICollectionService>();
            collectionService.Setup(repo => repo.RecordExistsAsync(collectionId)).Returns(Task.FromResult(true));

            var reosurceService = new Mock<IResourceService>();
            reosurceService.Setup(repo => repo.GetAsync(resourceId)).Returns(this.GetTestResource(collectionId));

            var requestService = new Mock<IRequestService>();

            var controller = new RequestsController(collectionService.Object, reosurceService.Object, requestService.Object);

            // Act
            var result = await controller.Put(collectionId, resourceId, "id1", null);

            // Assert
            var viewResult = Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task CanPatchRequest()
        {
            const string collectionId = "collectionId";
            const string resourceId = "resourceId";

            // Arrange
            var collectionService = new Mock<ICollectionService>();
            collectionService.Setup(repo => repo.RecordExistsAsync(collectionId)).Returns(Task.FromResult(true));

            var reosurceService = new Mock<IResourceService>();
            reosurceService.Setup(repo => repo.GetAsync(resourceId)).Returns(this.GetTestResource(collectionId));

            Request testRequest = new Request
            {
                Template = "Test Template",
                ResourceId = resourceId,
            };
            string testID = "id1";
            var requestService = new Mock<IRequestService>();
            requestService.Setup(repo => repo.AddOrUpdateAsync(testRequest)).Returns(Task.FromResult(testRequest));
            requestService.Setup(repo => repo.GetAsync(testID)).Returns(Task.FromResult(testRequest));

            var controller = new RequestsController(collectionService.Object, reosurceService.Object, requestService.Object);

            // Act
            var result = await controller.Patch(collectionId, resourceId, testID, new JsonPatchDocument<RequestUpdateDto>());

            // Assert
            var viewResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task CannotPatchRequestWhenEmptyRequest()
        {
            const string collectionId = "collectionId";
            const string resourceId = "resourceId";

            // Arrange
            var collectionService = new Mock<ICollectionService>();
            collectionService.Setup(repo => repo.RecordExistsAsync(collectionId)).Returns(Task.FromResult(true));

            var reosurceService = new Mock<IResourceService>();
            reosurceService.Setup(repo => repo.GetAsync(resourceId)).Returns(this.GetTestResource(collectionId));

            var requestService = new Mock<IRequestService>();

            var controller = new RequestsController(collectionService.Object, reosurceService.Object, requestService.Object);

            // Act
            var result = await controller.Patch(collectionId, resourceId, "id1", null);

            // Assert
            var viewResult = Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task CanDeleteRequest()
        {
            const string collectionId = "collectionId";
            const string resourceId = "resourceId";

            // Arrange
            var collectionService = new Mock<ICollectionService>();
            collectionService.Setup(repo => repo.RecordExistsAsync(collectionId)).Returns(Task.FromResult(true));

            var reosurceService = new Mock<IResourceService>();
            reosurceService.Setup(repo => repo.GetAsync(resourceId)).Returns(this.GetTestResource(collectionId));

            Request testRequest = new Request
            {
                ID = "id1",
                ResourceId = resourceId,
                Template = "Test Template",
            };
            var requestService = new Mock<IRequestService>();
            requestService.Setup(repo => repo.GetAsync(testRequest.ID)).Returns(Task.FromResult(testRequest));

            var controller = new RequestsController(collectionService.Object, reosurceService.Object, requestService.Object);

            // Act
            var result = await controller.Delete(collectionId, resourceId, "id1");

            // Assert
            var viewResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task CannotDeleteRequestWhenIdNotExists()
        {
            const string collectionId = "collectionId";
            const string resourceId = "resourceId";

            // Arrange
            var collectionService = new Mock<ICollectionService>();
            collectionService.Setup(repo => repo.RecordExistsAsync(collectionId)).Returns(Task.FromResult(true));

            var reosurceService = new Mock<IResourceService>();
            reosurceService.Setup(repo => repo.GetAsync(resourceId)).Returns(this.GetTestResource(collectionId));

            var requestService = new Mock<IRequestService>();

            var controller = new RequestsController(collectionService.Object, reosurceService.Object, requestService.Object);

            // Act
            var result = await controller.Delete(collectionId, resourceId, "id1");

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

        private Task<Resource> GetTestResource(string collectionId)
        {
            var mock = new Resource
            {
                CreatedTimestamp = new DateTime(2016, 7, 2),
                ID = "id1",
                Name = "Test One",
                Path = "test",
                CollectionId = collectionId,
            };

            return Task.FromResult(mock);
        }
    }
}
