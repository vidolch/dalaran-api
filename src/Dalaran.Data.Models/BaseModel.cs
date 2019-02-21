// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace Dalaran.Data.Models
{
    using System;

    public class BaseModel
    {
        public string ID { get; set; }

        public DateTimeOffset CreatedTimestamp { get; set; }

        public DateTimeOffset? UpdatedTimestamp { get; set; }
    }
}
