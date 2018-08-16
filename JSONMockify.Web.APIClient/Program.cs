// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace JSONMockify.Web.APIClient
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    public class Program
    {
        public static void Main(string[] args)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.Custom.json", optional: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
                .AddCommandLine(args)

                // .AddEnvironmentSecrets(args)
                .Build();

            BuildWebHostBuilder(args, configuration).Build().Run();
        }

        public static IWebHostBuilder BuildWebHostBuilder(string[] args, IConfiguration configuration) =>
            WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(configuration)
                .UseStartup<Startup>();
    }
}
