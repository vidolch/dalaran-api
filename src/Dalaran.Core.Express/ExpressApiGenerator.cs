// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace Dalaran.Core.Express
{
    using System.IO;
    using System.IO.Compression;
    using System.Text;
    using Dalaran.Core.Domain;
    using Dalaran.Core.Domain.Code;

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
                        switch (request.HttpMethod)
                        {
                            case Data.Models.HttpMethods.GET:
                                routes.AddLine($"app.get('/{resource.Path}', function (req, res) {{");
                                routes.IncreaseTabs().AddLine($"res.send('{request.Template}')");
                                routes.DecreaseTabs().AddLine($"}});");
                                break;
                            case Data.Models.HttpMethods.HEAD:
                                routes.AddLine($"app.head('/{resource.Path}', function (req, res) {{");
                                routes.IncreaseTabs().AddLine($"res.send('{request.Template}')");
                                routes.DecreaseTabs().AddLine($"}});");
                                break;
                            case Data.Models.HttpMethods.POST:
                                routes.AddLine($"app.post('/{resource.Path}', function (req, res) {{");
                                routes.IncreaseTabs().AddLine($"res.send('{request.Template}')");
                                routes.DecreaseTabs().AddLine($"}});");
                                break;
                            case Data.Models.HttpMethods.PUT:
                                routes.AddLine($"app.put('/{resource.Path}', function (req, res) {{");
                                routes.IncreaseTabs().AddLine($"res.send('{request.Template}')");
                                routes.DecreaseTabs().AddLine($"}});");
                                break;
                            case Data.Models.HttpMethods.DELETE:
                                routes.AddLine($"app.delete('/{resource.Path}', function (req, res) {{");
                                routes.IncreaseTabs().AddLine($"res.send('{request.Template}')");
                                routes.DecreaseTabs().AddLine($"}});");
                                break;
                            case Data.Models.HttpMethods.CONNECT:
                                routes.AddLine($"app.connect('/{resource.Path}', function (req, res) {{");
                                routes.IncreaseTabs().AddLine($"res.send('{request.Template}')");
                                routes.DecreaseTabs().AddLine($"}});");
                                break;
                            case Data.Models.HttpMethods.OPTIONS:
                                routes.AddLine($"app.options('/{resource.Path}', function (req, res) {{");
                                routes.IncreaseTabs().AddLine($"res.send('{request.Template}')");
                                routes.DecreaseTabs().AddLine($"}});");
                                break;
                            case Data.Models.HttpMethods.TRACE:
                                routes.AddLine($"app.trance('/{resource.Path}', function (req, res) {{");
                                routes.IncreaseTabs().AddLine($"res.send('{request.Template}')");
                                routes.DecreaseTabs().AddLine($"}});");
                                break;
                            case Data.Models.HttpMethods.PATCH:
                                routes.AddLine($"app.patch('/{resource.Path}', function (req, res) {{");
                                routes.IncreaseTabs().AddLine($"res.send('{request.Template}')");
                                routes.DecreaseTabs().AddLine($"}});");
                                break;
                            default:
                                break;
                        }

                        routes.AddLine();
                    }
                }
            }

            return routes;
        }
    }
}
