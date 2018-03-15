using System.ComponentModel.DataAnnotations;

namespace JSONMockifyAPI.Data.Models
{
    public class BaseModel
    {
        [Key]
        public int ID { get; set; }
    }
}
