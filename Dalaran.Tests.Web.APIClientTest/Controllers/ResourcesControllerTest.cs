// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace Tests.Web.APIClientTest.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
            // Arrange
            var resourceService = new Mock<IResourceService>();
            resourceService.Setup(repo => repo.GetAllAsync(null, 0, 20)).Returns(this.GetTestResources());
            var controller = new ResourcesController(resourceService.Object);

            // Act
            var result = await controller.GetAsync();

            // Assert
            var viewResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<ResourceListDto>(
                viewResult.Value);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task CanGetNoMocksAsync()
        {
            // Arrange
            var resourceService = new Mock<IResourceService>();
            resourceService.Setup(repo => repo.GetAllAsync(null, 0, 20)).Returns(Task.FromResult((new List<Resource>().AsEnumerable(), (long)0)));
            var controller = new ResourcesController(resourceService.Object);

            // Act
            var result = await controller.GetAsync();

            // Assert
            var viewResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<ResourceListDto>(
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
            var resourceService = new Mock<IResourceService>();
            resourceService.Setup(repo => repo.GetAsync(testId)).Returns(Task.FromResult(new Resource
            {
                ID = testId,
                Name = testTemplate,
                CreatedTimestamp = testCreated
            }));

            var controller = new ResourcesController(resourceService.Object);

            // Act
            var result = await controller.GetAsync(testId);

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
            // Arrange
            string testId = "id1";
            var resourceService = new Mock<IResourceService>();
            resourceService.Setup(repo => repo.GetAsync(testId)).Returns(Task.FromResult((Resource)null));

            var controller = new ResourcesController(resourceService.Object);

            // Act
            var result = await controller.GetAsync(testId);

            // Assert
            var viewResult = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CanCreateResourceAsync()
        {
            // Arrange
            Resource testResource = new Resource
            {
                Name = "Test Template"
            };
            ResourceUpdateDto testResourceDto = new ResourceUpdateDto
            {
                Name = "Test Template"
            };
            var resourceService = new Mock<IResourceService>();
            resourceService.Setup(repo => repo.AddOrUpdateAsync(testResource)).Returns(Task.FromResult(testResource));

            var controller = new ResourcesController(resourceService.Object);

            // Act
            var result = await controller.PostAsync(testResourceDto);

            // Assert
            var viewResult = Assert.IsType<CreatedAtRouteResult>(result);
            var model = Assert.IsAssignableFrom<ResourceDto>(
                viewResult.Value);
            Assert.Equal(testResource.Name, model.Name);
        }

        [Fact]
        public async Task CannotCreateResourceWhenEmptyResourceAsync()
        {
            // Arrange
            var resourceService = new Mock<IResourceService>();

            var controller = new ResourcesController(resourceService.Object);

            // Act
            var result = await controller.PostAsync(null);

            // Assert
            var viewResult = Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task CanPutResourceAsync()
        {
            // Arrange
            ResourceUpdateDto testResourceDto = new ResourceUpdateDto
            {
                Name = "Test Template"
            };
            Resource testResource = new Resource
            {
                Name = "Test Template"
            };
            string testID = "id1";
            var resourceService = new Mock<IResourceService>();
            resourceService.Setup(repo => repo.AddOrUpdateAsync(testResource)).Returns(Task.FromResult(testResource));
            resourceService.Setup(repo => repo.RecordExistsAsync(testID)).Returns(Task.FromResult(true));

            var controller = new ResourcesController(resourceService.Object);

            // Act
            var result = await controller.PutAsync(testID, testResourceDto);

            // Assert
            var viewResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task CannotPutResourceWhenEmptyRequestAsync()
        {
            // Arrange
            var resourceService = new Mock<IResourceService>();

            var controller = new ResourcesController(resourceService.Object);

            // Act
            var result = await controller.PutAsync("id1", null);

            // Assert
            var viewResult = Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task CanPatchResourceAsync()
        {
            // Arrange
            Resource testResource = new Resource
            {
                Name = "Test Template"
            };
            string testID = "id1";
            var resourceService = new Mock<IResourceService>();
            resourceService.Setup(repo => repo.AddOrUpdateAsync(testResource)).Returns(Task.FromResult(testResource));
            resourceService.Setup(repo => repo.RecordExistsAsync(testID)).Returns(Task.FromResult(true));

            var controller = new ResourcesController(resourceService.Object);

            // Act
            var result = await controller.PatchAsync(testID, new JsonPatchDocument<ResourceUpdateDto>());

            // Assert
            var viewResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task CannotPatchResourceWhenEmptyRequestAsync()
        {
            // Arrange
            var resourceService = new Mock<IResourceService>();

            var controller = new ResourcesController(resourceService.Object);

            // Act
            var result = await controller.PatchAsync("id1", null);

            // Assert
            var viewResult = Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task CanDeleteResourceAsync()
        {
            // Arrange
            Resource testResource = new Resource
            {
                Name = "Test Template"
            };
            string testID = "id1";
            var resourceService = new Mock<IResourceService>();
            resourceService.Setup(repo => repo.RecordExistsAsync(testID)).Returns(Task.FromResult(true));

            var controller = new ResourcesController(resourceService.Object);

            // Act
            var result = await controller.DeleteAsync("id1");

            // Assert
            var viewResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task CannotDeleteResourceWhenIDNotExistsAsync()
        {
            // Arrange
            var resourceService = new Mock<IResourceService>();

            var controller = new ResourcesController(resourceService.Object);

            // Act
            var result = await controller.DeleteAsync("id1");

            // Assert
            var viewResult = Assert.IsType<NotFoundResult>(result);
        }

        private Task<(IEnumerable<Resource>, long)> GetTestResources()
        {
            var mocks = new List<Resource>();
            mocks.Add(new Resource()
            {
                ID = "id1",
                Name = "Test One"
            });
            mocks.Add(new Resource()
            {
                ID = "id2",
                Name = "Test Two"
            });
            return Task.FromResult((mocks.AsEnumerable(), (long)mocks.Count));
        }
    }
}
