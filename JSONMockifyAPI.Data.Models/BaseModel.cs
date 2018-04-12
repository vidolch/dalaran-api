using System;
using System.ComponentModel.DataAnnotations;

namespace JSONMockifyAPI.Data.Models
{
    public class BaseModel
    {
        [Key]
        public Guid ID { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
