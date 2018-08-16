﻿// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace JSONMockifyAPI.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class BaseModel
    {
        [Key]
        public string ID { get; set; }

        public DateTimeOffset CreatedTimestamp { get; set; }

        public DateTimeOffset? UpdatedTimestamp { get; set; }
    }
}
