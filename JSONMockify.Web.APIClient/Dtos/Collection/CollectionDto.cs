// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace JSONMockify.Web.APIClient.Dtos.Collection
{
    using System;
    using JSONMockifyAPI.Data.Models;

    public class CollectionDto : BaseDto
    {
        public CollectionDto(Collection collection)
        {
            this.Name = collection.Name;
            this.ID = collection.ID;
            this.CreatedTimestamp = collection.CreatedTimestamp;
            this.UpdatedTimestamp = collection.UpdatedTimestamp;
        }

        public string Name { get; set; }

        public DateTimeOffset CreatedTimestamp { get; set; }

        public DateTimeOffset? UpdatedTimestamp { get; set; }
    }
}
