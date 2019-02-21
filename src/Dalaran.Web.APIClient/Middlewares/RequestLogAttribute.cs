// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace Dalaran.Web.APIClient.Middlewares
{
    using System;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Serilog;

    [AttributeUsage(AttributeTargets.Delegate)]
    public class RequestLogAttribute : TypeFilterAttribute
    {
        public RequestLogAttribute()
            : base(typeof(RequestLogFilter))
        {
        }

        private class RequestLogFilter : IActionFilter
        {
            public void OnActionExecuting(ActionExecutingContext context)
            {
                var request = context.HttpContext.Request;
                var contentType = request.ContentType != null ? request.ContentType + " " : string.Empty;
                var url = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";

                var message = $"[{context.HttpContext.TraceIdentifier}] Starting {request.Method} {url} {contentType}to action {context.ActionDescriptor.DisplayName}";

                if (request.Method == "GET" || request.Method == "HEAD")
                {
                    Log.Debug(message);
                }
                else
                {
                    Log.Information(message);
                }
            }

            public void OnActionExecuted(ActionExecutedContext context)
            {
                var request = context.HttpContext.Request;
                var response = context.HttpContext.Response;
                var contentType = response.ContentType != null ? response.ContentType + " " : string.Empty;
                var url = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";

                var message = $"[{context.HttpContext.TraceIdentifier}] Finished {response.StatusCode} {url} {contentType}{context.HttpContext.TraceIdentifier}";

                if (request.Method == "GET" || request.Method == "HEAD")
                {
                    Log.Debug(message);
                }
                else
                {
                    Log.Information(message);
                }
            }
        }
    }
}
