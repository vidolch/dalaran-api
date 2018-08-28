// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace Tests.Web.APIClientTest.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Dalaran.Web.APIClient.Controllers;
    using Dalaran.Web.APIClient.Dtos.Collection;
    using Dalaran.Data.Models;
    using Dalaran.Services.Data.Contracts;
    using Microsoft.AspNetCore.JsonPatch;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using Xunit;

    public class CollectionsControllerTest
    {
        [Fact]
        public async Task CanGetAllCollectionsAsync()
        {
            // Arrange
            var collectionService = new Mock<ICollectionService>();
            collectionService.Setup(repo => repo.GetAllAsync(null, 0, 20)).Returns(this.GetTestCollections());
            var controller = new CollectionsController(collectionService.Object);

            // Act
            var result = await controller.GetAsync();

            // Assert
            var viewResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<CollectionListDto>(
                viewResult.Value);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task CanGetNoMocksAsync()
        {
            // Arrange
            var collectionService = new Mock<ICollectionService>();
            collectionService.Setup(repo => repo.GetAllAsync(null, 0, 20)).Returns(Task.FromResult((new List<Collection>().AsEnumerable(), (long)0)));
            var controller = new CollectionsController(collectionService.Object);

            // Act
            var result = await controller.GetAsync();

            // Assert
            var viewResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<CollectionListDto>(
                viewResult.Value);
            Assert.Empty(model);
        }

        [Fact]
        public async Task CanGetMockByIdAsync()
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
            var result = await controller.GetAsync(testId);

            // Assert
            var viewResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<CollectionDto>(
                viewResult.Value);
            Assert.Equal(testId, model.ID);
            Assert.Equal(testTemplate, model.Name);
            Assert.Equal(testCreated, model.CreatedTimestamp);
        }

        [Fact]
        public async Task CanGetNoMockByIdAsync()
        {
            // Arrange
            string testId = "id1";
            var collectionService = new Mock<ICollectionService>();
            collectionService.Setup(repo => repo.GetAsync(testId)).Returns(Task.FromResult((Collection)null));

            var controller = new CollectionsController(collectionService.Object);

            // Act
            var result = await controller.GetAsync(testId);

            // Assert
            var viewResult = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CanCreateCollectionAsync()
        {
            // Arrange
            Collection testCollection = new Collection
            {
                Name = "Test Template"
            };

            CollectionUpdateDto testCollectionDto = new CollectionUpdateDto
            {
                Name = "Test Template"
            };

            var collectionService = new Mock<ICollectionService>();
            collectionService.Setup(repo => repo.AddOrUpdateAsync(testCollection)).Returns(Task.FromResult(testCollection));

            var controller = new CollectionsController(collectionService.Object);

            // Act
            var result = await controller.PostAsync(testCollectionDto);

            // Assert
            var viewResult = Assert.IsType<CreatedAtRouteResult>(result);
            var model = Assert.IsAssignableFrom<CollectionDto>(
                viewResult.Value);
            Assert.Equal(testCollection.Name, model.Name);
        }

        [Fact]
        public async Task CannotCreateCollectionWhenEmptyRequestAsync()
        {
            // Arrange
            var collectionService = new Mock<ICollectionService>();

            var controller = new CollectionsController(collectionService.Object);

            // Act
            var result = await controller.PostAsync(null);

            // Assert
            var viewResult = Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task CanPutCollectionAsync()
        {
            // Arrange
            Collection testCollection = new Collection
            {
                Name = "Test Template"
            };

            CollectionUpdateDto testCollectionDto = new CollectionUpdateDto
            {
                Name = "Test Template"
            };

            string testID = "id1";
            var collectionService = new Mock<ICollectionService>();
            collectionService.Setup(repo => repo.AddOrUpdateAsync(testCollection)).Returns(Task.FromResult(testCollection));
            collectionService.Setup(repo => repo.RecordExistsAsync(testID)).Returns(Task.FromResult(true));

            var controller = new CollectionsController(collectionService.Object);

            // Act
            var result = await controller.PutAsync(testID, testCollectionDto);

            // Assert
            var viewResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task CannotPutCollectionWhenEmptyRequestAsync()
        {
            // Arrange
            var collectionService = new Mock<ICollectionService>();

            var controller = new CollectionsController(collectionService.Object);

            // Act
            var result = await controller.PutAsync("id1", null);

            // Assert
            var viewResult = Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task CanPatchCollectionAsync()
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
            var result = await controller.PatchAsync(testID, new JsonPatchDocument<CollectionUpdateDto>());

            // Assert
            var viewResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task CannotPatchCollectionWhenEmptyRequestAsync()
        {
            // Arrange
            var collectionService = new Mock<ICollectionService>();

            var controller = new CollectionsController(collectionService.Object);

            // Act
            var result = await controller.PatchAsync("id1", null);

            // Assert
            var viewResult = Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task CanDeleteCollectionAsync()
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
            var result = await controller.DeleteAsync("id1");

            // Assert
            var viewResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task CannotDeleteCollectionWhenIDNotExistsAsync()
        {
            // Arrange
            var collectionService = new Mock<ICollectionService>();

            var controller = new CollectionsController(collectionService.Object);

            // Act
            var result = await controller.DeleteAsync("id1");

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
