// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace Tests.Web.APIClientTest.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using JSONMockify.Web.APIClient.Controllers;
    using JSONMockifyAPI.Data.Models;
    using JSONMockifyAPI.Services.Data.Contracts;
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
            var requestRepo = new Mock<IRequestService>();
            requestRepo.Setup(repo => repo.GetAllAsync(null, 0, 20)).Returns(this.GetTestJSONMocks());
            var controller = new RequestsController(requestRepo.Object);

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
            var requestRepo = new Mock<IRequestService>();
            requestRepo.Setup(repo => repo.GetAllAsync(null, 0, 20)).Returns(Task.FromResult((new List<Request>().AsEnumerable(), (long)0)));
            var controller = new RequestsController(requestRepo.Object);

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
            var requestRepo = new Mock<IRequestService>();
            requestRepo.Setup(repo => repo.GetAsync(testId)).Returns(Task.FromResult(new Request
            {
                ID = testId,
                CreatedTimestamp = testCreated,
                Template = testTemplate
            }));

            var controller = new RequestsController(requestRepo.Object);

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
            var requestRepo = new Mock<IRequestService>();
            requestRepo.Setup(repo => repo.GetAsync(testId)).Returns(Task.FromResult((Request)null));

            var controller = new RequestsController(requestRepo.Object);

            // Act
            var result = controller.Get(testId);

            // Assert
            var viewResult = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void CanCreateJSONMock()
        {
            // Arrange
            Request testJSONMock = new Request
            {
                Template = "Test Template"
            };
            var requestRepo = new Mock<IRequestService>();
            requestRepo.Setup(repo => repo.AddOrUpdateAsync(testJSONMock)).Returns(Task.FromResult(testJSONMock));

            var controller = new RequestsController(requestRepo.Object);

            // Act
            var result = controller.Post(testJSONMock);

            // Assert
            var viewResult = Assert.IsType<CreatedAtRouteResult>(result);
            var model = Assert.IsAssignableFrom<Request>(
                viewResult.Value);
            Assert.Equal(testJSONMock.Template, model.Template);
        }

        [Fact]
        public void CannotCreateJSONMockWhenEmptyRequest()
        {
            // Arrange
            var requestRepo = new Mock<IRequestService>();

            var controller = new RequestsController(requestRepo.Object);

            // Act
            var result = controller.Post(null);

            // Assert
            var viewResult = Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void CanPutJSONMock()
        {
            // Arrange
            Request testJSONMock = new Request
            {
                Template = "Test Template"
            };
            string testID = "id1";
            var requestRepo = new Mock<IRequestService>();
            requestRepo.Setup(repo => repo.AddOrUpdateAsync(testJSONMock)).Returns(Task.FromResult(testJSONMock));
            requestRepo.Setup(repo => repo.RecordExistsAsync(testID)).Returns(Task.FromResult(true));

            var controller = new RequestsController(requestRepo.Object);

            // Act
            var result = controller.Put(testID, testJSONMock);

            // Assert
            var viewResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void CannotPutJSONMockWhenEmptyRequest()
        {
            // Arrange
            var requestRepo = new Mock<IRequestService>();

            var controller = new RequestsController(requestRepo.Object);

            // Act
            var result = controller.Put("id1", null);

            // Assert
            var viewResult = Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void CanPatchJSONMock()
        {
            // Arrange
            Request testJSONMock = new Request
            {
                Template = "Test Template"
            };
            string testID = "id1";
            var requestRepo = new Mock<IRequestService>();
            requestRepo.Setup(repo => repo.AddOrUpdateAsync(testJSONMock)).Returns(Task.FromResult(testJSONMock));
            requestRepo.Setup(repo => repo.RecordExistsAsync(testID)).Returns(Task.FromResult(true));

            var controller = new RequestsController(requestRepo.Object);

            // Act
            var result = controller.Patch(testID, new JsonPatchDocument<Request>());

            // Assert
            var viewResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void CannotPatchJSONMockWhenEmptyRequest()
        {
            // Arrange
            var requestRepo = new Mock<IRequestService>();

            var controller = new RequestsController(requestRepo.Object);

            // Act
            var result = controller.Patch("id1", null);

            // Assert
            var viewResult = Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void CanDeleteJSONMock()
        {
            // Arrange
            Request testJSONMock = new Request
            {
                Template = "Test Template"
            };
            string testID = "id1";
            var requestRepo = new Mock<IRequestService>();
            requestRepo.Setup(repo => repo.RecordExistsAsync(testID)).Returns(Task.FromResult(true));

            var controller = new RequestsController(requestRepo.Object);

            // Act
            var result = controller.Delete("id1");

            // Assert
            var viewResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void CannotDeleteJSONMockWhenIDNotExists()
        {
            // Arrange
            var requestRepo = new Mock<IRequestService>();

            var controller = new RequestsController(requestRepo.Object);

            // Act
            var result = controller.Delete("id1");

            // Assert
            var viewResult = Assert.IsType<NotFoundResult>(result);
        }

        private Task<(IEnumerable<Request>, long)> GetTestJSONMocks()
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
