﻿// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace Dalaran.Console.Commands.Login
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;

    public sealed class LoopbackHttpListener : IDisposable
    {
        private const int DefaultTimeout = 5 * 60; // 5 minutes

        private readonly IWebHost host;
        private readonly TaskCompletionSource<string> source = new TaskCompletionSource<string>();
        private readonly string url;

        public LoopbackHttpListener(int port, string path = null)
        {
            path = path ?? string.Empty;

            if (path.StartsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                path = path.Substring(1);
            }

            this.url = $"http://127.0.0.1:{port}/{path}";
            this.host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls(this.url)
                .Configure(this.Configure)
                .Build();

            this.host.Start();
        }

#pragma warning disable CA1056 // Uri properties should not be strings
        public string Url => this.url;
#pragma warning restore CA1056 // Uri properties should not be strings

        public Task<string> WaitForCallbackAsync(int timeoutInSeconds = DefaultTimeout)
        {
            Task.Run(
                async () =>
                {
                    await Task.Delay(timeoutInSeconds * 1000).ConfigureAwait(false);
                    this.source.TrySetCanceled();
                });

            return this.source.Task;
        }

        public void Dispose()
        {
            Task.Run(
                async () =>
                {
                    await Task.Delay(500).ConfigureAwait(false);
                    this.host.Dispose();
                });
        }

        private void Configure(IApplicationBuilder app)
        {
            app.Run(
                async ctx =>
                {
                    if (ctx.Request.Method == HttpMethods.Get)
                    {
                        this.SetResult(ctx.Request.QueryString.Value, ctx);
                    }
                    else if (ctx.Request.Method == HttpMethods.Post)
                    {
                        if (!ctx.Request.ContentType.Equals("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase))
                        {
                            ctx.Response.StatusCode = (int)HttpStatusCode.UnsupportedMediaType;
                        }
                        else
                        {
                            using (var sr = new StreamReader(ctx.Request.Body, Encoding.UTF8))
                            {
                                var body = await sr.ReadToEndAsync().ConfigureAwait(false);
                                this.SetResult(body, ctx);
                            }
                        }
                    }
                    else
                    {
                        ctx.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                    }
                });
        }

        private void SetResult(string value, HttpContext ctx)
        {
            try
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                ctx.Response.ContentType = "text/html";
                ctx.Response.WriteAsync("<h1>You can now return to the application.</h1>");
                ctx.Response.Body.Flush();

                this.source.TrySetResult(value);
            }
            catch
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                ctx.Response.ContentType = "text/html";
                ctx.Response.WriteAsync("<h1>Invalid request.</h1>");
                ctx.Response.Body.Flush();
            }
        }
    }
}
