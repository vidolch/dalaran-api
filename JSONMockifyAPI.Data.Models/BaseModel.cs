using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace JSONMockifyAPI.Data.Models
{
    public class BaseModel
    {
        // mongo fix
        [BsonId]
        public ObjectId _id { get; set; }

        [Key]
        public Guid ID { get; set; }
        public DateTimeOffset CreatedTimestamp { get; set; }
        public DateTimeOffset? UpdatedTimestamp { get; set; }
    }
}
