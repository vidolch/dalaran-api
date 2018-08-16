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

    public class JSONMocksControllerTest
    {
        [Fact]
        public void CanGetAllMocks()
        {
            // Arrange
            var mockRepo = new Mock<IJSONMockService>();
            mockRepo.Setup(repo => repo.GetAllAsync(null, 0, 20)).Returns(this.GetTestJSONMocks());
            var controller = new JSONMocksController(mockRepo.Object);

            // Act
            var result = controller.Get();

            // Assert
            var viewResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<JSONMock>>(
                viewResult.Value);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public void CanGetNoMocks()
        {
            // Arrange
            var mockRepo = new Mock<IJSONMockService>();
            mockRepo.Setup(repo => repo.GetAllAsync(null, 0, 20)).Returns(Task.FromResult((new List<JSONMock>().AsEnumerable(), (long)0)));
            var controller = new JSONMocksController(mockRepo.Object);

            // Act
            var result = controller.Get();

            // Assert
            var viewResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<JSONMock>>(
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
            var mockRepo = new Mock<IJSONMockService>();
            mockRepo.Setup(repo => repo.GetAsync(testId)).Returns(Task.FromResult(new JSONMock
            {
                ID = testId,
                CreatedTimestamp = testCreated,
                Template = testTemplate
            }));

            var controller = new JSONMocksController(mockRepo.Object);

            // Act
            var result = controller.Get(testId);

            // Assert
            var viewResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<JSONMock>(
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
            var mockRepo = new Mock<IJSONMockService>();
            mockRepo.Setup(repo => repo.GetAsync(testId)).Returns(Task.FromResult((JSONMock)null));

            var controller = new JSONMocksController(mockRepo.Object);

            // Act
            var result = controller.Get(testId);

            // Assert
            var viewResult = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void CanCreateJSONMock()
        {
            // Arrange
            JSONMock testJSONMock = new JSONMock
            {
                Template = "Test Template"
            };
            var mockRepo = new Mock<IJSONMockService>();
            mockRepo.Setup(repo => repo.AddOrUpdateAsync(string.Empty, testJSONMock)).Returns(Task.FromResult(testJSONMock));

            var controller = new JSONMocksController(mockRepo.Object);

            // Act
            var result = controller.Post(testJSONMock);

            // Assert
            var viewResult = Assert.IsType<CreatedAtRouteResult>(result);
            var model = Assert.IsAssignableFrom<JSONMock>(
                viewResult.Value);
            Assert.Equal(testJSONMock.Template, model.Template);
        }

        [Fact]
        public void CannotCreateJSONMockWhenEmptyRequest()
        {
            // Arrange
            var mockRepo = new Mock<IJSONMockService>();

            var controller = new JSONMocksController(mockRepo.Object);

            // Act
            var result = controller.Post(null);

            // Assert
            var viewResult = Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void CanPutJSONMock()
        {
            // Arrange
            JSONMock testJSONMock = new JSONMock
            {
                Template = "Test Template"
            };
            string testID = "id1";
            var mockRepo = new Mock<IJSONMockService>();
            mockRepo.Setup(repo => repo.AddOrUpdateAsync(string.Empty, testJSONMock)).Returns(Task.FromResult(testJSONMock));
            mockRepo.Setup(repo => repo.RecordExistsAsync(testID)).Returns(Task.FromResult(true));

            var controller = new JSONMocksController(mockRepo.Object);

            // Act
            var result = controller.Put(testID, testJSONMock);

            // Assert
            var viewResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void CannotPutJSONMockWhenEmptyRequest()
        {
            // Arrange
            var mockRepo = new Mock<IJSONMockService>();

            var controller = new JSONMocksController(mockRepo.Object);

            // Act
            var result = controller.Put("id1", null);

            // Assert
            var viewResult = Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void CanPatchJSONMock()
        {
            // Arrange
            JSONMock testJSONMock = new JSONMock
            {
                Template = "Test Template"
            };
            string testID = "id1";
            var mockRepo = new Mock<IJSONMockService>();
            mockRepo.Setup(repo => repo.AddOrUpdateAsync(string.Empty, testJSONMock)).Returns(Task.FromResult(testJSONMock));
            mockRepo.Setup(repo => repo.RecordExistsAsync(testID)).Returns(Task.FromResult(true));

            var controller = new JSONMocksController(mockRepo.Object);

            // Act
            var result = controller.Patch(testID, new JsonPatchDocument<JSONMock>());

            // Assert
            var viewResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void CannotPatchJSONMockWhenEmptyRequest()
        {
            // Arrange
            var mockRepo = new Mock<IJSONMockService>();

            var controller = new JSONMocksController(mockRepo.Object);

            // Act
            var result = controller.Patch("id1", null);

            // Assert
            var viewResult = Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void CanDeleteJSONMock()
        {
            // Arrange
            JSONMock testJSONMock = new JSONMock
            {
                Template = "Test Template"
            };
            string testID = "id1";
            var mockRepo = new Mock<IJSONMockService>();
            mockRepo.Setup(repo => repo.RecordExistsAsync(testID)).Returns(Task.FromResult(true));

            var controller = new JSONMocksController(mockRepo.Object);

            // Act
            var result = controller.Delete("id1");

            // Assert
            var viewResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void CannotDeleteJSONMockWhenIDNotExists()
        {
            // Arrange
            var mockRepo = new Mock<IJSONMockService>();

            var controller = new JSONMocksController(mockRepo.Object);

            // Act
            var result = controller.Delete("id1");

            // Assert
            var viewResult = Assert.IsType<NotFoundResult>(result);
        }

        private Task<(IEnumerable<JSONMock>, long)> GetTestJSONMocks()
        {
            var mocks = new List<JSONMock>();
            mocks.Add(new JSONMock()
            {
                CreatedTimestamp = new DateTime(2016, 7, 2),
                ID = "id1",
                Template = "Test One"
            });
            mocks.Add(new JSONMock()
            {
                CreatedTimestamp = new DateTime(2016, 7, 3),
                ID = "id2",
                Template = "Test Two"
            });
            return Task.FromResult((mocks.AsEnumerable(), (long)mocks.Count));
        }
    }
}
