// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace Tests.Web.APIClientTest.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Dalaran.Web.APIClient.Controllers;
    using Dalaran.Data.Models;
    using Dalaran.Services.Data.Contracts;
    using Microsoft.AspNetCore.JsonPatch;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using Xunit;

    public class RequestsControllerTest
    {
        [Fact]
        public void CanGetAllMocks()
        {
            // Arrange
            var requestService = new Mock<IRequestService>();
            requestService.Setup(repo => repo.GetAllAsync(null, 0, 20)).Returns(this.GetTestRequests());
            var controller = new RequestsController(requestService.Object);

            // Act
            var result = controller.Get();

            // Assert
            var viewResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Request>>(
                viewResult.Value);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public void CanGetNoMocks()
        {
            // Arrange
            var requestService = new Mock<IRequestService>();
            requestService.Setup(repo => repo.GetAllAsync(null, 0, 20)).Returns(Task.FromResult((new List<Request>().AsEnumerable(), (long)0)));
            var controller = new RequestsController(requestService.Object);

            // Act
            var result = controller.Get();

            // Assert
            var viewResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Request>>(
                viewResult.Value);
            Assert.Empty(model);
        }

        [Fact]
        public void CanGetMockById()
        {
            // Arrange
            string testId = "id1";
            DateTimeOffset testCreated = new DateTime(2017, 12, 2);
            string testTemplate = "Test Template";
            var requestService = new Mock<IRequestService>();
            requestService.Setup(repo => repo.GetAsync(testId)).Returns(Task.FromResult(new Request
            {
                ID = testId,
                CreatedTimestamp = testCreated,
                Template = testTemplate
            }));

            var controller = new RequestsController(requestService.Object);

            // Act
            var result = controller.Get(testId);

            // Assert
            var viewResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<Request>(
                viewResult.Value);
            Assert.Equal(testId, model.ID);
            Assert.Equal(testTemplate, model.Template);
            Assert.Equal(testCreated, model.CreatedTimestamp);
        }

        [Fact]
        public void CanGetNoMockById()
        {
            // Arrange
            string testId = "id1";
            var requestService = new Mock<IRequestService>();
            requestService.Setup(repo => repo.GetAsync(testId)).Returns(Task.FromResult((Request)null));

            var controller = new RequestsController(requestService.Object);

            // Act
            var result = controller.Get(testId);

            // Assert
            var viewResult = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void CanCreateRequest()
        {
            // Arrange
            Request testRequest = new Request
            {
                Template = "Test Template"
            };
            var requestService = new Mock<IRequestService>();
            requestService.Setup(repo => repo.AddOrUpdateAsync(testRequest)).Returns(Task.FromResult(testRequest));

            var controller = new RequestsController(requestService.Object);

            // Act
            var result = controller.Post(testRequest);

            // Assert
            var viewResult = Assert.IsType<CreatedAtRouteResult>(result);
            var model = Assert.IsAssignableFrom<Request>(
                viewResult.Value);
            Assert.Equal(testRequest.Template, model.Template);
        }

        [Fact]
        public void CannotCreateRequestWhenEmptyRequest()
        {
            // Arrange
            var requestService = new Mock<IRequestService>();

            var controller = new RequestsController(requestService.Object);

            // Act
            var result = controller.Post(null);

            // Assert
            var viewResult = Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void CanPutRequest()
        {
            // Arrange
            Request testRequest = new Request
            {
                Template = "Test Template"
            };
            string testID = "id1";
            var requestService = new Mock<IRequestService>();
            requestService.Setup(repo => repo.AddOrUpdateAsync(testRequest)).Returns(Task.FromResult(testRequest));
            requestService.Setup(repo => repo.RecordExistsAsync(testID)).Returns(Task.FromResult(true));

            var controller = new RequestsController(requestService.Object);

            // Act
            var result = controller.Put(testID, testRequest);

            // Assert
            var viewResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void CannotPutRequestWhenEmptyRequest()
        {
            // Arrange
            var requestService = new Mock<IRequestService>();

            var controller = new RequestsController(requestService.Object);

            // Act
            var result = controller.Put("id1", null);

            // Assert
            var viewResult = Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void CanPatchRequest()
        {
            // Arrange
            Request testRequest = new Request
            {
                Template = "Test Template"
            };
            string testID = "id1";
            var requestService = new Mock<IRequestService>();
            requestService.Setup(repo => repo.AddOrUpdateAsync(testRequest)).Returns(Task.FromResult(testRequest));
            requestService.Setup(repo => repo.RecordExistsAsync(testID)).Returns(Task.FromResult(true));

            var controller = new RequestsController(requestService.Object);

            // Act
            var result = controller.Patch(testID, new JsonPatchDocument<Request>());

            // Assert
            var viewResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void CannotPatchRequestWhenEmptyRequest()
        {
            // Arrange
            var requestService = new Mock<IRequestService>();

            var controller = new RequestsController(requestService.Object);

            // Act
            var result = controller.Patch("id1", null);

            // Assert
            var viewResult = Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void CanDeleteRequest()
        {
            // Arrange
            Request testRequest = new Request
            {
                Template = "Test Template"
            };
            string testID = "id1";
            var requestService = new Mock<IRequestService>();
            requestService.Setup(repo => repo.RecordExistsAsync(testID)).Returns(Task.FromResult(true));

            var controller = new RequestsController(requestService.Object);

            // Act
            var result = controller.Delete("id1");

            // Assert
            var viewResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void CannotDeleteRequestWhenIDNotExists()
        {
            // Arrange
            var requestService = new Mock<IRequestService>();

            var controller = new RequestsController(requestService.Object);

            // Act
            var result = controller.Delete("id1");

            // Assert
            var viewResult = Assert.IsType<NotFoundResult>(result);
        }

        private Task<(IEnumerable<Request>, long)> GetTestRequests()
        {
            var mocks = new List<Request>();
            mocks.Add(new Request()
            {
                CreatedTimestamp = new DateTime(2016, 7, 2),
                ID = "id1",
                Template = "Test One"
            });
            mocks.Add(new Request()
            {
                CreatedTimestamp = new DateTime(2016, 7, 3),
                ID = "id2",
                Template = "Test Two"
            });
            return Task.FromResult((mocks.AsEnumerable(), (long)mocks.Count));
        }
    }
}
