// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace JSONMockify.Web.APIClient.Dtos.Collection
{
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    public class CollectionUpdateDto : BaseDto
    {
        [Required]
        [MinLength(3)]
        public string Name { get; set; }
    }
}
