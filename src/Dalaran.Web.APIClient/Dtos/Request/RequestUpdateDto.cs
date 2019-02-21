// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace Dalaran.Web.APIClient.Dtos.Request
{
    using System.ComponentModel.DataAnnotations;
    using Dalaran.Data.Models;

    public class RequestUpdateDto : BaseDto
    {
        [Required]
        [MinLength(3)]
        public string Name { get; set; }

        [Required]
        [MinLength(2)]
        public string Template { get; set; }

        [Required]
        public HttpMethods HttpMethod { get; set; }
    }
}