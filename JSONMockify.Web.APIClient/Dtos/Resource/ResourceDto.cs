// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace JSONMockify.Web.APIClient.Dtos.Resource
{
    using System;
    using JSONMockifyAPI.Data.Models;

    public class ResourceDto : BaseDto
    {
        public ResourceDto(Resource resource)
        {
            this.Name = resource.Name;
            this.Path = resource.Path;
            this.ID = resource.ID;
            this.CreatedTimestamp = resource.CreatedTimestamp;
            this.UpdatedTimestamp = resource.UpdatedTimestamp;
        }

        public string Name { get; set; }

        public string Path { get; set; }

        public DateTimeOffset CreatedTimestamp { get; set; }

        public DateTimeOffset? UpdatedTimestamp { get; set; }
    }
}
