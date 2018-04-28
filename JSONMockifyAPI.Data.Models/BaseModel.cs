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
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
