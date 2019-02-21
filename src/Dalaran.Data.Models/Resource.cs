// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace Dalaran.Data.Models
{
    using System.Collections.Generic;

    public class Resource : BaseModel
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public string CollectionId { get; set; }

        public IEnumerable<Request> Requests { get; set; }
    }
}
