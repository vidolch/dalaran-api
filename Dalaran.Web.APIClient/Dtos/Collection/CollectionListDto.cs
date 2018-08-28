// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace Dalaran.Web.APIClient.Dtos.Collection
{
    using System.Collections.Generic;

    public class CollectionListDto : BaseListDto<CollectionDto>
    {
        public CollectionListDto(long page, long totalSize, IEnumerable<CollectionDto> items)
            : base(page, totalSize, items)
        {
        }
    }
}
