// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace JSONMockify.Web.APIClient.Controllers
{
    using System.Threading.Tasks;
    using JSONMockifyAPI.Data.Models;
    using JSONMockifyAPI.Data.Repositories.Interfaces;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/fake")]
    [EnableCors("UI")]
    public class FakeController : Controller
    {
        private readonly IRequestRepository requestRepository;

        public FakeController(IRequestRepository requestRepository)
        {
            this.requestRepository = requestRepository;
        }

        [HttpGet("{id}", Name = "GetFake")]
        public async Task<IActionResult> GetAsync(string id)
        {
            var result = await this.requestRepository.GetForMethodAsync(id, HttpMethods.GET);

            if (result == null)
            {
                return this.NotFound();
            }

            return this.Ok(result);
        }

        [HttpPost("{id}", Name = "PostFake")]
        public async Task<IActionResult> PostAsync(string id, [FromBody]object content)
        {
            var result = await this.requestRepository.GetForMethodAsync(id, HttpMethods.POST);

            if (result == null)
            {
                return this.NotFound();
            }

            return this.Ok(result);
        }
    }
}
