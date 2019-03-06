// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace Dalaran.Core.Express
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using Dalaran.Core.Domain;
    using Dalaran.Core.Domain.Code;
    using Dalaran.Core.Express.Code;

    public class ExpressApiGenerator : IApiGenerator
    {
        private readonly string templatePath;

        public ExpressApiGenerator(string templatePath)
        {
            this.templatePath = templatePath;
        }

        public Api GenerateApi(ApiConfiguration configuration)
        {
            var packageJson = Encoding.UTF8.GetBytes(File.ReadAllText(Path.Combine(this.templatePath, "package.json.tmpl")));
            var indexJs = File.ReadAllText(Path.Combine(this.templatePath, "index.js.tmpl"));

            var routes = this.GetRoutes(configuration);

            indexJs = indexJs.Replace("-{routes}-", routes.ToString());

            var indexJsBytes = Encoding.UTF8.GetBytes(indexJs);

            using (var fileStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(fileStream, ZipArchiveMode.Create, true))
                {
                    var fileName = "package.json";
                    var fileName2 = "index.js";

                    var zipArchiveEntry = archive.CreateEntry(fileName, CompressionLevel.Fastest);
                    using (var zipStream = zipArchiveEntry.Open())
                    {
                        zipStream.Write(packageJson, 0, packageJson.Length);
                    }

                    zipArchiveEntry = archive.CreateEntry(fileName2, CompressionLevel.Fastest);
                    using (var zipStream = zipArchiveEntry.Open())
                    {
                        zipStream.Write(indexJsBytes, 0, indexJsBytes.Length);
                    }
                }

                return new ExpressApi { Archive = fileStream.ToArray() };
            }
        }

        private Script GetRoutes(ApiConfiguration configuration)
        {
            var routes = new Script();

            foreach (var collection in configuration.Collections)
            {
                foreach (var resource in collection.Resources)
                {
                    routes.AddLine($"// Resource {resource.Name}").AddLine();

                    foreach (var request in resource.Requests)
                    {
                        RequestMethod requestMethod;
                        switch (request.HttpMethod)
                        {
                            case Data.Models.HttpMethods.GET:
                                requestMethod = new GetRequestMethod();
                                break;
                            case Data.Models.HttpMethods.HEAD:
                                requestMethod = new HeadRequestMethod();
                                break;
                            case Data.Models.HttpMethods.POST:
                                requestMethod = new PostRequestMethod();
                                break;
                            case Data.Models.HttpMethods.PUT:
                                requestMethod = new PutRequestMethod();
                                break;
                            case Data.Models.HttpMethods.DELETE:
                                requestMethod = new DeleteRequestMethod();
                                break;
                            case Data.Models.HttpMethods.CONNECT:
                                requestMethod = new ConnectRequestMethod();
                                break;
                            case Data.Models.HttpMethods.OPTIONS:
                                requestMethod = new OptionsRequestMethod();
                                break;
                            case Data.Models.HttpMethods.TRACE:
                                requestMethod = new TraceRequestMethod();
                                break;
                            case Data.Models.HttpMethods.PATCH:
                                requestMethod = new PatchRequestMethod();
                                break;
                            default:
                                throw new ArgumentException("Unsupported Http Method.");
                        }

                        routes = requestMethod.GetRequest(routes, resource, request);

                        routes.AddLine();
                    }
                }
            }

            return routes;
        }
    }
}
