// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace Dalaran.Web.APIClient.Dtos.Collection
{
    using System;
    using System.Collections;
    using Dalaran.Data.Models;
    using Dalaran.Web.APIClient.Dtos.Resource;

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

        public ResourceListDto Resources { get; set; }

        public DateTimeOffset CreatedTimestamp { get; set; }

        public DateTimeOffset? UpdatedTimestamp { get; set; }
    }
}
