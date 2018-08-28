// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace Dalaran.Services.Data
{
    using Dalaran.Data.Models;
    using Dalaran.Data.Repositories.Interfaces;
    using Dalaran.Services.Data.Contracts;

    public class ResourceService : DataService<Resource>, IResourceService
    {
        public ResourceService(IResourceRepository repository)
            : base(repository)
        {
        }
    }
}
