// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace Dalaran.Web.APIClient.Dtos
{
    using Dalaran.Web.APIClient.Dtos.Request;
    using RequestModel = Dalaran.Data.Models.Request;

    public static class DtoExtensions
    {
        public static RequestModel ToModel(this RequestUpdateDto requestDto, string id, string resourceId)
        {
            return new RequestModel
            {
                ID = id,
                Name = requestDto.Name,
                Path = requestDto.Path,
                Template = requestDto.Template,
                HttpMethod = requestDto.HttpMethod,
                ResponseType = requestDto.ResponseType,
                ResponseCode = requestDto.ResponseCode,
                ResourceId = resourceId,
            };
        }
    }
}
