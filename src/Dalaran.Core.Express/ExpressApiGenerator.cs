// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace Dalaran.Core.Express
{
    using System.IO;
    using System.IO.Compression;
    using System.Text;
    using Dalaran.Core.Domain;

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

            var routes = new StringBuilder();

            foreach (var collection in configuration.Collections)
            {
                foreach (var resource in collection.Resources)
                {
                    foreach (var request in resource.Requests)
                    {
                        switch (request.HttpMethod)
                        {
                            case Data.Models.HttpMethods.GET:
                                routes.AppendLine($"app.get('/{resource.Path}', function (req, res) {{");
                                routes.AppendLine($"res.send('{request.Template}')");
                                routes.AppendLine($"}});");
                                break;
                            case Data.Models.HttpMethods.POST:
                                routes.AppendLine($"app.post('/{resource.Path}', function (req, res) {{");
                                routes.AppendLine($"res.send('{request.Template}')");
                                routes.AppendLine($"}});");
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            indexJs = indexJs.Replace("-{routes}-", routes.ToString());

            var indexJsBytes = Encoding.UTF8.GetBytes(indexJs);

            using (var fileStream = new FileStream(@"D:\Projects\JSONMockify\test\test.zip", FileMode.CreateNew))
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
            }
            return default(Api);
        }
    }
}
