// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace Dalaran.Data.Models
{
    using System.Collections.Generic;

    public class Collection : BaseModel
    {
        public Collection()
        {
            this.Resources = new List<Resource>();
        }

        public string Name { get; set; }

        public ICollection<Resource> Resources { get; set; }
    }
}
