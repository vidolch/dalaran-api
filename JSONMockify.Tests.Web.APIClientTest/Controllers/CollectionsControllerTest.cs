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

    public class CollectionsControllerTest
    {
        [Fact]
        public void CanGetAllCollections()
        {
            // Arrange
            var collectionService = new Mock<ICollectionService>();
            collectionService.Setup(repo => repo.GetAllAsync(null, 0, 20)).Returns(this.GetTestCollections());
            var controller = new CollectionsController(collectionService.Object);

            // Act
            var result = controller.Get();

            // Assert
            var viewResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Collection>>(
                viewResult.Value);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public void CanGetNoMocks()
        {
            // Arrange
            var collectionService = new Mock<ICollectionService>();
            collectionService.Setup(repo => repo.GetAllAsync(null, 0, 20)).Returns(Task.FromResult((new List<Collection>().AsEnumerable(), (long)0)));
            var controller = new CollectionsController(collectionService.Object);

            // Act
            var result = controller.Get();

            // Assert
            var viewResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Collection>>(
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
            var collectionService = new Mock<ICollectionService>();
            collectionService.Setup(repo => repo.GetAsync(testId)).Returns(Task.FromResult(new Collection
            {
                ID = testId,
                Name = testTemplate,
                CreatedTimestamp = testCreated
            }));

            var controller = new CollectionsController(collectionService.Object);

            // Act
            var result = controller.Get(testId);

            // Assert
            var viewResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<Collection>(
                viewResult.Value);
            Assert.Equal(testId, model.ID);
            Assert.Equal(testTemplate, model.Name);
            Assert.Equal(testCreated, model.CreatedTimestamp);
        }

        [Fact]
        public void CanGetNoMockById()
        {
            // Arrange
            string testId = "id1";
            var collectionService = new Mock<ICollectionService>();
            collectionService.Setup(repo => repo.GetAsync(testId)).Returns(Task.FromResult((Collection)null));

            var controller = new CollectionsController(collectionService.Object);

            // Act
            var result = controller.Get(testId);

            // Assert
            var viewResult = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void CanCreateCollection()
        {
            // Arrange
            Collection testCollection = new Collection
            {
                Name = "Test Template"
            };
            var collectionService = new Mock<ICollectionService>();
            collectionService.Setup(repo => repo.AddOrUpdateAsync(testCollection)).Returns(Task.FromResult(testCollection));

            var controller = new CollectionsController(collectionService.Object);

            // Act
            var result = controller.Post(testCollection);

            // Assert
            var viewResult = Assert.IsType<CreatedAtRouteResult>(result);
            var model = Assert.IsAssignableFrom<Collection>(
                viewResult.Value);
            Assert.Equal(testCollection.Name, model.Name);
        }

        [Fact]
        public void CannotCreateCollectionWhenEmptyRequest()
        {
            // Arrange
            var collectionService = new Mock<ICollectionService>();

            var controller = new CollectionsController(collectionService.Object);

            // Act
            var result = controller.Post(null);

            // Assert
            var viewResult = Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void CanPutCollection()
        {
            // Arrange
            Collection testCollection = new Collection
            {
                Name = "Test Template"
            };
            string testID = "id1";
            var collectionService = new Mock<ICollectionService>();
            collectionService.Setup(repo => repo.AddOrUpdateAsync(testCollection)).Returns(Task.FromResult(testCollection));
            collectionService.Setup(repo => repo.RecordExistsAsync(testID)).Returns(Task.FromResult(true));

            var controller = new CollectionsController(collectionService.Object);

            // Act
            var result = controller.Put(testID, testCollection);

            // Assert
            var viewResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void CannotPutCollectionWhenEmptyRequest()
        {
            // Arrange
            var collectionService = new Mock<ICollectionService>();

            var controller = new CollectionsController(collectionService.Object);

            // Act
            var result = controller.Put("id1", null);

            // Assert
            var viewResult = Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void CanPatchCollection()
        {
            // Arrange
            Collection testCollection = new Collection
            {
                Name = "Test Template"
            };
            string testID = "id1";
            var collectionService = new Mock<ICollectionService>();
            collectionService.Setup(repo => repo.AddOrUpdateAsync(testCollection)).Returns(Task.FromResult(testCollection));
            collectionService.Setup(repo => repo.RecordExistsAsync(testID)).Returns(Task.FromResult(true));

            var controller = new CollectionsController(collectionService.Object);

            // Act
            var result = controller.Patch(testID, new JsonPatchDocument<Collection>());

            // Assert
            var viewResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void CannotPatchCollectionWhenEmptyRequest()
        {
            // Arrange
            var collectionService = new Mock<ICollectionService>();

            var controller = new CollectionsController(collectionService.Object);

            // Act
            var result = controller.Patch("id1", null);

            // Assert
            var viewResult = Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void CanDeleteCollection()
        {
            // Arrange
            Collection testCollection = new Collection
            {
                Name = "Test Template"
            };
            string testID = "id1";
            var collectionService = new Mock<ICollectionService>();
            collectionService.Setup(repo => repo.RecordExistsAsync(testID)).Returns(Task.FromResult(true));

            var controller = new CollectionsController(collectionService.Object);

            // Act
            var result = controller.Delete("id1");

            // Assert
            var viewResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void CannotDeleteCollectionWhenIDNotExists()
        {
            // Arrange
            var collectionService = new Mock<ICollectionService>();

            var controller = new CollectionsController(collectionService.Object);

            // Act
            var result = controller.Delete("id1");

            // Assert
            var viewResult = Assert.IsType<NotFoundResult>(result);
        }

        private Task<(IEnumerable<Collection>, long)> GetTestCollections()
        {
            var mocks = new List<Collection>();
            mocks.Add(new Collection()
            {
                ID = "id1",
                Name = "Test One"
            });
            mocks.Add(new Collection()
            {
                ID = "id2",
                Name = "Test Two"
            });
            return Task.FromResult((mocks.AsEnumerable(), (long)mocks.Count));
        }
    }
}
