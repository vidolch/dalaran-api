﻿using JSONMockify.Web.APIClient.Controllers;
using JSONMockifyAPI.Data.Models;
using JSONMockifyAPI.Services.Data.Contracts;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.JsonPatch;

namespace Tests.Web.APIClientTest.Controllers
{
    public class JSONMocksControllerTest
    {
        [Fact]
        public void CanGetAllMocks()
        {
            // Arrange
            var mockRepo = new Mock<IJSONMockService>();
            mockRepo.Setup(repo => repo.GetAll()).Returns(GetTestJSONMocks());
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
            mockRepo.Setup(repo => repo.GetAll()).Returns(new List<JSONMock>());
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
            Guid testGuid = new Guid();
            DateTimeOffset testCreated = new DateTime(2017, 12, 2);
            string testTemplate = "Test Template";
            var mockRepo = new Mock<IJSONMockService>();
            mockRepo.Setup(repo => repo.Get(testGuid)).Returns(new JSONMock
            {
                ID = testGuid,
                CreatedAt = testCreated,
                Template = testTemplate
            });

            var controller = new JSONMocksController(mockRepo.Object);

            // Act
            var result = controller.Get(testGuid);

            // Assert
            var viewResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<JSONMock>(
                viewResult.Value);
            Assert.Equal(testGuid, model.ID);
            Assert.Equal(testTemplate, model.Template);
            Assert.Equal(testCreated, model.CreatedAt);
        }

        [Fact]
        public void CanGetNoMockById()
        {
            // Arrange
            Guid testGuid = new Guid();
            var mockRepo = new Mock<IJSONMockService>();
            mockRepo.Setup(repo => repo.Get(testGuid)).Returns((JSONMock)null);

            var controller = new JSONMocksController(mockRepo.Object);

            // Act
            var result = controller.Get(testGuid);

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
            mockRepo.Setup(repo => repo.Create(testJSONMock)).Returns(testJSONMock);

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
            Guid testID = new Guid();
            var mockRepo = new Mock<IJSONMockService>();
            mockRepo.Setup(repo => repo.Update(testJSONMock)).Returns(testJSONMock);
            mockRepo.Setup(repo => repo.RecordExists(testID)).Returns(true);

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
            var result = controller.Put(new Guid(), null);

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
            Guid testID = new Guid();
            var mockRepo = new Mock<IJSONMockService>();
            mockRepo.Setup(repo => repo.Update(testJSONMock)).Returns(testJSONMock);
            mockRepo.Setup(repo => repo.RecordExists(testID)).Returns(true);

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
            var result = controller.Patch(new Guid(), null);

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
            Guid testID = new Guid();
            var mockRepo = new Mock<IJSONMockService>();
            mockRepo.Setup(repo => repo.Update(testJSONMock)).Returns(testJSONMock);
            mockRepo.Setup(repo => repo.RecordExists(testID)).Returns(true);

            var controller = new JSONMocksController(mockRepo.Object);

            // Act
            var result = controller.Delete(testID);

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
            var result = controller.Delete(new Guid());

            // Assert
            var viewResult = Assert.IsType<NotFoundResult>(result);
        }

        private List<JSONMock> GetTestJSONMocks()
        {
            var mocks = new List<JSONMock>();
            mocks.Add(new JSONMock()
            {
                CreatedAt = new DateTime(2016, 7, 2),
                ID = new Guid(),
                Template = "Test One"
            });
            mocks.Add(new JSONMock()
            {
                CreatedAt = new DateTime(2016, 7, 3),
                ID = new Guid(),
                Template = "Test Two"
            });
            return mocks;
        }
    }
}
