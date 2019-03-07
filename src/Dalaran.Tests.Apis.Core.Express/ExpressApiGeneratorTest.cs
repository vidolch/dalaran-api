// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace Dalaran.Tests.Apis.Core.Express
{
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Net;
    using Dalaran.Core.Express;
    using Dalaran.Data.Models;
    using Xunit;

    public class ExpressApiGeneratorTest
    {
        [Fact]
        public void CanGetAllCollectionsAsync()
        {
            // Arrange
            var baseDir = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.Parent.Parent.Parent;

            var apiGenerator = new ExpressApiGenerator(Path.Combine(baseDir.FullName, Path.Combine("templates", "express")));

            var collection = new Collection
            {
                ID = "collection1",
                Name = "Test collection",
                Resources = new List<Resource>
                {
                    new Resource
                    {
                        ID = "resource1",
                        Name = "Test Resource",
                        Path = "resource1",
                        Requests = new List<Request>
                        {
                            new Request
                            {
                                ID = "request1",
                                Template = "Template",
                                HttpMethod = HttpMethods.GET,
                                ResponseCode = HttpStatusCode.Accepted,
                                ResponseType = ResponseTypes.JSON,
                                Name = "Request 1",
                            },
                            new Request
                            {
                                ID = "request2",
                                Template = "Template",
                                HttpMethod = HttpMethods.POST,
                                ResponseCode = HttpStatusCode.Forbidden,
                                ResponseType = ResponseTypes.JSON,
                                Name = "Request 2",
                            },
                        },
                    },
                    new Resource
                    {
                        ID = "resource2",
                        Name = "Test Resource 2",
                        Path = "resource2",
                        Requests = new List<Request>
                        {
                            new Request
                            {
                                ID = "request3",
                                Template = "Template",
                                HttpMethod = HttpMethods.HEAD,
                                ResponseCode = HttpStatusCode.NotFound,
                                ResponseType = ResponseTypes.XML,
                                Name = "Request 3",
                            },
                            new Request
                            {
                                ID = "request4",
                                Template = "Template",
                                HttpMethod = HttpMethods.PATCH,
                                ResponseCode = HttpStatusCode.Redirect,
                                ResponseType = ResponseTypes.JSON,
                                Name = "Request 4",
                            },
                            new Request
                            {
                                ID = "request5",
                                Template = "Template",
                                HttpMethod = HttpMethods.PUT,
                                ResponseCode = HttpStatusCode.Unused,
                                ResponseType = ResponseTypes.XML,
                                Name = "Request 5",
                            },
                        },
                    },
                },
            };

            var collection2 = new Collection
            {
                ID = "collection1",
                Name = "Test collection",
                Resources = new List<Resource>
                {
                    new Resource
                    {
                        ID = "resource3",
                        Name = "Test Resource",
                        Path = "resource1",
                        Requests = new List<Request>
                        {
                            new Request
                            {
                                ID = "request6",
                                Template = "Template",
                                HttpMethod = HttpMethods.DELETE,
                                ResponseCode = HttpStatusCode.Forbidden,
                                ResponseType = ResponseTypes.XML,
                                Name = "Request 6",
                            },
                            new Request
                            {
                                ID = "request7",
                                Template = "Template",
                                HttpMethod = HttpMethods.CONNECT,
                                ResponseCode = HttpStatusCode.UnsupportedMediaType,
                                ResponseType = ResponseTypes.JSON,
                                Name = "Request 7",
                            },
                            new Request
                            {
                                ID = "request8",
                                Template = "Template",
                                HttpMethod = HttpMethods.TRACE,
                                ResponseCode = HttpStatusCode.BadGateway,
                                ResponseType = ResponseTypes.XML,
                                Name = "Request 8",
                            },
                            new Request
                            {
                                ID = "request9",
                                Template = "Template",
                                HttpMethod = HttpMethods.OPTIONS,
                                ResponseCode = HttpStatusCode.Continue,
                                ResponseType = ResponseTypes.JSON,
                                Name = "Request 9",
                            },
                        },
                    },
                },
            };

            // Act
            var result = apiGenerator.GenerateApi(new Dalaran.Core.Domain.ApiConfiguration
            {
                Collections = new List<Collection>
                {
                    collection,
                    collection2,
                },
            });

            // Assert
            var expectedJsFile = @"var express = require('express');
var app = express();

// Resource Test Resource

app.get('/resource1', function (req, res) {
	res.status(202);
	res.set('Content-Type', 'application/json');
	var template = 'Template';
	res.send(template)
});

app.post('/resource1', function (req, res) {
	res.status(403);
	res.set('Content-Type', 'application/json');
	var template = 'Template';
	res.send(template)
});

// Resource Test Resource 2

app.head('/resource2', function (req, res) {
	res.status(404);
	res.set('Content-Type', 'text/xml');
	var template = 'Template';
	res.send(template)
});

app.patch('/resource2', function (req, res) {
	res.status(302);
	res.set('Content-Type', 'application/json');
	var template = 'Template';
	res.send(template)
});

app.put('/resource2', function (req, res) {
	res.status(306);
	res.set('Content-Type', 'text/xml');
	var template = 'Template';
	res.send(template)
});

// Resource Test Resource

app.delete('/resource1', function (req, res) {
	res.status(403);
	res.set('Content-Type', 'text/xml');
	var template = 'Template';
	res.send(template)
});

app.connect('/resource1', function (req, res) {
	res.status(415);
	res.set('Content-Type', 'application/json');
	var template = 'Template';
	res.send(template)
});

app.trace('/resource1', function (req, res) {
	res.status(502);
	res.set('Content-Type', 'text/xml');
	var template = 'Template';
	res.send(template)
});

app.options('/resource1', function (req, res) {
	res.status(100);
	res.set('Content-Type', 'application/json');
	var template = 'Template';
	res.send(template)
});



app.listen(3000, function () {
    console.log('Example app listening on port 3000!')
});";

            using (var stream = new MemoryStream(result.Archive))
            {
                using (var zip = new ZipArchive(stream, ZipArchiveMode.Read))
                {
                    foreach (var entry in zip.Entries)
                    {
                        if (entry.FullName == "index.js")
                        {
                            using (var entryStream = new StreamReader(entry.Open()))
                            {
                                var resultString = entryStream.ReadToEnd();
                                Assert.Equal(expectedJsFile, resultString);
                            }
                        }
                    }
                }
            }
        }
    }
}
