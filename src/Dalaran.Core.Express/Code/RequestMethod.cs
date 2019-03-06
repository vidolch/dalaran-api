// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace Dalaran.Core.Express.Code
{
    using Dalaran.Core.Domain.Code;
    using Dalaran.Data.Models;

    internal abstract class RequestMethod
    {
        protected abstract string Method { get; }

        public virtual Script GetRequest(Script script, Resource resource, Request request)
        {
            script.AddLine($"app.{this.Method}('/{resource.Path}', function (req, res) {{");
            script.IncreaseTabs()
                .AddLine($"res.status({request.ResponseCode.ToString("d")});")
                .AddLine($"res.set('Content-Type', '{this.GetResponseType(request)});")
                .AddLine($"res.send('{request.Template}')");
            script.DecreaseTabs().AddLine($"}});");

            return script;
        }

        private object GetResponseType(Request request)
        {
            return request.ResponseType == ResponseTypes.JSON ? "application/json" : "text/xml";
        }
    }
}
