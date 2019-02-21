// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace Dalaran.Web.APIClient.Dtos.Resource
{
    using System.ComponentModel.DataAnnotations;

    public class ResourceUpdateDto : BaseDto
    {
        [Required]
        [MinLength(3)]
        public string Name { get; set; }

        [Required]
        [MinLength(1)]
        public string Path { get; set; }

        [Required]
        [MinLength(1)]
        public string CollectionId { get; set; }
    }
}