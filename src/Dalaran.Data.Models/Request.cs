// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace Dalaran.Data.Models
{
    public class Request : BaseModel
    {
        public string Name { get; set; }

        public string Template { get; set; }

        public HttpMethods HttpMethod { get; set; }

        public ResponseTypes ResponseType { get; set; }

        public string ResourceId { get; set; }
    }
}
