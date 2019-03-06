// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace Dalaran.Web.APIClient.Dtos.Request
{
    using System;
    using System.Net;
    using Dalaran.Data.Models;

    public class RequestDto : BaseDto
    {
        public RequestDto(Request request)
        {
            this.ID = request.ID;
            this.Name = request.Name;
            this.Path = request.Path;
            this.Template = request.Template;
            this.HttpMethod = request.HttpMethod;
            this.ResponseType = request.ResponseType;
            this.ResponseCode = request.ResponseCode;
            this.ResourceId = request.ResourceId;
            this.CreatedTimestamp = request.CreatedTimestamp;
            this.UpdatedTimestamp = request.UpdatedTimestamp;
        }

        public string Name { get; set; }

        public string Path { get; set; }

        public string Template { get; set; }

        public HttpMethods HttpMethod { get; set; }

        public ResponseTypes ResponseType { get; set; }

        public HttpStatusCode ResponseCode { get; set; }

        public string ResourceId { get; set; }

        public DateTimeOffset CreatedTimestamp { get; set; }

        public DateTimeOffset? UpdatedTimestamp { get; set; }
    }
}