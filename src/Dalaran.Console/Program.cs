// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace Dalaran.Console
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Dalaran.Console.Authorization;
    using Dalaran.Console.Commands;
    using Dalaran.Console.Persistence;
    using McMaster.Extensions.CommandLineUtils;
    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Serilog;
    using Serilog.Events;

    public class Program
    {
        private readonly IConsole console;
        private readonly IDataProtectionProvider provider;

        public Program(IConsole console, IDataProtectionProvider provider)
        {
            this.console = console;
            this.provider = provider;
        }

        public static Task<int> Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.Custom.json", optional: true)
                .AddCommandLine(args)
                .AddUserSecrets<Program>()
                .Build();

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Async(a => a.Console())
                .MinimumLevel.Override("Microsoft", LogEventLevel.Debug)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            DebugHelper.HandleDebugSwitch(ref args);

            JsonConvert.DefaultSettings =
                () =>
                new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() }
                };

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddDataProtection();
            serviceCollection.AddHttpClient<HttpClient>();

            serviceCollection.AddSingleton<TokenEndpointProvider>(factory =>
            {
                var authority = configuration.GetValue<string>("Api-Authority");
                return new TokenEndpointProvider(authority);
            });

            serviceCollection.AddSingleton<ClientTokenProvider>(factory =>
            {
                var clientName = configuration.GetValue<string>("Client-Name");
                var clientSecret = configuration.GetValue<string>("Client-Secret");
                var tokenEndpointProvider = factory.GetService<TokenEndpointProvider>();

                return new ClientTokenProvider(tokenEndpointProvider, clientName, clientSecret);
            });

            serviceCollection.AddSingleton(PhysicalConsole.Singleton);

            var services = serviceCollection.BuildServiceProvider();

            var instance = ActivatorUtilities.CreateInstance<Program>(services);
            return instance.TryRunAsync(args, services);
        }

        public async Task<int> TryRunAsync(string[] args, ServiceProvider services)
        {
            CommandLineOptions options;
            try
            {
                options = CommandLineOptions.Parse(args, this.console, services);
            }
            catch (CommandParsingException ex)
            {
                new ConsoleReporter(this.console).Warn(ex.Message);
                return 1;
            }

            if (options == null)
            {
                return 1;
            }

            if (options.Help.HasValue())
            {
                return 2;
            }

            if (options.Command == null)
            {
                return 3;
            }

            var repository = new CommandDataRepository(this.provider);

            var reporter = new ConsoleReporter(this.console, options.Verbose.HasValue(), false);
            var context = new CommandContext(this.console, reporter, repository);

            try
            {
                await options.Command.ExecuteAsync(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                reporter.Error(ex.Message);
                return 500;
            }
            finally
            {
                this.console.ResetColor();
            }

            Log.CloseAndFlush();

            return 0;
        }
    }
}
