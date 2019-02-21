// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace Dalaran.Core.Domain
{
    using System.Collections.Generic;
    using Dalaran.Data.Models;

    public class ApiConfiguration
    {
        public IEnumerable<Collection> Collections { get; set; }
    }
}