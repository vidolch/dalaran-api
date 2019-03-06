// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace Dalaran.Core.Express.Code
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Dalaran.Core.Domain.Code;
    using Dalaran.Data.Models;

    internal abstract class RequestMethod
    {
        protected abstract string Method { get; }

        public virtual Script GetRequest(Script script, Resource resource, Request request)
        {
            var path = $"/{resource.Path.TrimStart('/')}";
            List<string> urlParams = null;

            if (!string.IsNullOrEmpty(request.Path))
            {
                path += $"/{request.Path.TrimStart('/')}";

                urlParams = Regex.Matches(path, @"\:\w+").Cast<Match>().Select(m => m.Value).ToList();
            }

            script.AddLine($"app.{this.Method}('{path}', function (req, res) {{");
            script.IncreaseTabs()
                .AddLine($"res.status({request.ResponseCode.ToString("d")});")
                .AddLine($"res.set('Content-Type', '{this.GetResponseType(request)}');")
                .AddLine($"var template = '{request.Template.Replace(Environment.NewLine, string.Empty)}';");

            if (urlParams != null && urlParams.Any())
            {
                foreach (var item in urlParams)
                {
                    script.AddLine($"template = template.replace('{{{item}}}', req.params.{item.TrimStart(':')});");
                }
            }

            script.AddLine($"res.send(template)");
            script.DecreaseTabs().AddLine($"}});");

            return script;
        }

        private object GetResponseType(Request request)
        {
            return request.ResponseType == ResponseTypes.JSON ? "application/json" : "text/xml";
        }
    }
}
