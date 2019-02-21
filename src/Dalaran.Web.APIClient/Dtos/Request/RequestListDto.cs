// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace Dalaran.Web.APIClient.Dtos.Request
{
    using System.Collections.Generic;

    public class RequestListDto : BaseListDto<RequestDto>
    {
        public RequestListDto(long page, long totalSize, IEnumerable<RequestDto> items)
            : base(page, totalSize, items)
        {
        }
    }
}