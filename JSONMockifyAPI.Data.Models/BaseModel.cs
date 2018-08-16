// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace JSONMockifyAPI.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    public class BaseModel
    {
        // mongo fix
        [BsonId]
#pragma warning disable SA1300 // Element must begin with upper-case letter
        public ObjectId _id { get; set; }
#pragma warning restore SA1300 // Element must begin with upper-case letter

        [Key]
        public Guid ID { get; set; }

        public DateTimeOffset CreatedTimestamp { get; set; }

        public DateTimeOffset? UpdatedTimestamp { get; set; }
    }
}
