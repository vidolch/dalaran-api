// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace JSONMockifyAPI.Data.Models
{
    using System.Collections.Generic;

    public class Resource : BaseModel
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public Collection Collection { get; set; }

        public virtual ICollection<Resource> Resources { get; set; }
    }
}
