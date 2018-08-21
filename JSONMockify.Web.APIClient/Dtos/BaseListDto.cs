// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace JSONMockify.Web.APIClient.Dtos
{
    using System.Collections;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    [JsonObject]
    public abstract class BaseListDto<T> : IEnumerable<T>
        where T : class
    {
        public BaseListDto(long page, long totalSize, IEnumerable<T> items)
        {
            this.Page = page;
            this.TotalSize = totalSize;
            this.Contents = new List<T>(items);
        }

        [JsonProperty]
        public IList<T> Contents { get; set; }

        [JsonProperty]
        public long Page { get; set; }

        [JsonProperty]
        public long Size => this.Contents.Count;

        [JsonProperty]
        public long TotalSize { get; set; }

        public IEnumerator<T> GetEnumerator()
        {
            return this.Contents.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
