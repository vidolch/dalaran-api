// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace Dalaran.Web.APIClient.Dtos.Resource
{
    using System.Collections.Generic;

    public class ResourceListDto : BaseListDto<ResourceDto>
    {
        public ResourceListDto(long page, long totalSize, IEnumerable<ResourceDto> items)
            : base(page, totalSize, items)
        {
        }
    }
}