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

    public class ResourcesControllerTest
    {
        [Fact]
        public void CanGetAllResources()
        {
            // Arrange
            var resourceService = new Mock<IResourceService>();
            resourceService.Setup(repo => repo.GetAllAsync(null, 0, 20)).Returns(this.GetTestResources());
            var controller = new ResourcesController(resourceService.Object);

            // Act
            var result = controller.Get();

            // Assert
            var viewResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Resource>>(
                viewResult.Value);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public void CanGetNoMocks()
        {
            // Arrange
            var resourceService = new Mock<IResourceService>();
            resourceService.Setup(repo => repo.GetAllAsync(null, 0, 20)).Returns(Task.FromResult((new List<Resource>().AsEnumerable(), (long)0)));
            var controller = new ResourcesController(resourceService.Object);

            // Act
            var result = controller.Get();

            // Assert
            var viewResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Resource>>(
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
            var resourceService = new Mock<IResourceService>();
            resourceService.Setup(repo => repo.GetAsync(testId)).Returns(Task.FromResult(new Resource
            {
                ID = testId,
                Name = testTemplate,
                CreatedTimestamp = testCreated
            }));

            var controller = new ResourcesController(resourceService.Object);

            // Act
            var result = controller.Get(testId);

            // Assert
            var viewResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<Resource>(
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
            var resourceService = new Mock<IResourceService>();
            resourceService.Setup(repo => repo.GetAsync(testId)).Returns(Task.FromResult((Resource)null));

            var controller = new ResourcesController(resourceService.Object);

            // Act
            var result = controller.Get(testId);

            // Assert
            var viewResult = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void CanCreateResource()
        {
            // Arrange
            Resource testResource = new Resource
            {
                Name = "Test Template"
            };
            var resourceService = new Mock<IResourceService>();
            resourceService.Setup(repo => repo.AddOrUpdateAsync(testResource)).Returns(Task.FromResult(testResource));

            var controller = new ResourcesController(resourceService.Object);

            // Act
            var result = controller.Post(testResource);

            // Assert
            var viewResult = Assert.IsType<CreatedAtRouteResult>(result);
            var model = Assert.IsAssignableFrom<Resource>(
                viewResult.Value);
            Assert.Equal(testResource.Name, model.Name);
        }

        [Fact]
        public void CannotCreateResourceWhenEmptyResource()
        {
            // Arrange
            var resourceService = new Mock<IResourceService>();

            var controller = new ResourcesController(resourceService.Object);

            // Act
            var result = controller.Post(null);

            // Assert
            var viewResult = Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void CanPutResource()
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
            var result = controller.Put(testID, testResource);

            // Assert
            var viewResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void CannotPutResourceWhenEmptyRequest()
        {
            // Arrange
            var resourceService = new Mock<IResourceService>();

            var controller = new ResourcesController(resourceService.Object);

            // Act
            var result = controller.Put("id1", null);

            // Assert
            var viewResult = Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void CanPatchResource()
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
            var result = controller.Patch(testID, new JsonPatchDocument<Resource>());

            // Assert
            var viewResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void CannotPatchResourceWhenEmptyRequest()
        {
            // Arrange
            var resourceService = new Mock<IResourceService>();

            var controller = new ResourcesController(resourceService.Object);

            // Act
            var result = controller.Patch("id1", null);

            // Assert
            var viewResult = Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void CanDeleteResource()
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
            var result = controller.Delete("id1");

            // Assert
            var viewResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void CannotDeleteResourceWhenIDNotExists()
        {
            // Arrange
            var resourceService = new Mock<IResourceService>();

            var controller = new ResourcesController(resourceService.Object);

            // Act
            var result = controller.Delete("id1");

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
