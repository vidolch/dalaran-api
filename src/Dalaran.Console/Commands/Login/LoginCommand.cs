// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace Dalaran.Console.Commands.Login
{
    using System;
    using System.Net.Http;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Dalaran.Console.Persistence;
    using IdentityModel.Client;
    using IdentityModel.OidcClient;
    using McMaster.Extensions.CommandLineUtils;
    using Newtonsoft.Json;

    internal class LoginCommand : ICommand
    {
        public const string DefaultAuthority = Consts.IdentityServer.Url;
        public const string DefaultService = Consts.IdentityServer.Api.Url;

        private Api api;

        private LoginCommand()
        {
        }

        public string Authority { get; private set; }

        public string Service { get; private set; }

        public bool ForceLogin { get; private set; }

        public static void Configure(CommandLineApplication app, CommandLineOptions options, IConsole console)
        {
            // description
            app.Description = "Log in to an authorization server";

            // arguments
            var argumentAuthority = app.Argument("authority", "The URL for the authorization server to log in to");
            var argumentService = app.Argument("service", "The service url to send requests to");

            // options
            var optionForce = app.Option("-f|--force", "Forces client to login, ignoring the existing context", CommandOptionType.NoValue);
            var optionReset = app.Option("-r|--reset", "Resets the authorization context", CommandOptionType.NoValue);
            app.HelpOption();

            // action (for this command)
            app.OnExecute(
               async () =>
                {
                    if (!string.IsNullOrEmpty(optionReset.Value()) && string.IsNullOrEmpty(argumentAuthority.Value))
                    {
                            // only --reset was specified
                            options.Command = new Reset();
                        return;
                    }

                    var authority = argumentAuthority.Value;
                    if (string.IsNullOrEmpty(authority))
                    {
                        authority = "http://localhost:5005";
                    }

                    // validate
                    if (!Uri.TryCreate(authority, UriKind.Absolute, out var authorityUri))
                    {
                        console.Error.WriteLine($"Invalid authority URL specified: {authority}.");
                        return;
                    }

                    var service = argumentService.Value;
                    if (string.IsNullOrEmpty(service))
                    {
                        service = DefaultService;
                    }

                        // validate
                        if (!Uri.TryCreate(service, UriKind.Absolute, out var serviceUri))
                    {
                        console.Error.WriteLine($"Invalid service URL specified: {service}.");
                        return;
                    }

                    var discoveryResponse = default(DiscoveryResponse);
                    using (var discoveryClient = new DiscoveryClient(authority) { Policy = new DiscoveryPolicy { ValidateIssuerName = false } })
                    {
                        discoveryResponse = await discoveryClient.GetAsync().ConfigureAwait(false);
                        if (discoveryResponse.IsError)
                        {
                            console.Error.WriteLine($"Discovery error: {discoveryResponse.Error}.");
                            return;
                        }
                    }

                    var apiUri = discoveryResponse.TryGetString("api_uri") ?? authority + "/api";

                    var apiResponse = default(Api);
                    using (var client = new HttpClient())
                    {
                        try
                        {
                            using (var response = client.GetAsync(new Uri(apiUri)).GetAwaiter().GetResult())
                            {
                                apiResponse = JsonConvert.DeserializeObject<Api>(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
                            }
                        }
                        catch (JsonReaderException)
                        {
                            console.Error.WriteLine($"Unable to connect to API at: {apiUri}.");
                            return;
                        }
                        catch (HttpRequestException)
                        {
                            console.Error.WriteLine($"Unable to connect to: {authority}.");
                            return;
                        }
                    }

                    if (apiResponse == null)
                    {
                        console.Error.WriteLine($"Invalid response from: {authority}.");
                        return;
                    }

                    options.Command = new LoginCommand
                    {
                        Authority = authority,
                        api = apiResponse,
                        Service = service,
                        ForceLogin = optionForce.HasValue()
                    };
                });
        }

        public async Task ExecuteAsync(CommandContext context)
        {
            context.Console.WriteLine($"Logging in to {this.Authority} ({this.api.Title} v{this.api.Version} running on {this.api.OS})...");

            var data = context.Repository.GetCommandData();
            if (!this.ForceLogin && data != null && data.Authority == this.Authority)
            {
                // already logged in?
                var discoveryResponse = default(DiscoveryResponse);
                using (var discoveryClient = new DiscoveryClient(this.Authority))
                {
                    discoveryResponse = await discoveryClient.GetAsync().ConfigureAwait(false);
                    if (!discoveryResponse.IsError)
                    {
                        using (var tokenClient = new TokenClient(discoveryResponse.TokenEndpoint, Consts.IdentityServer.Client.Id))
                        using (var refreshTokenHandler = new RefreshTokenHandler(tokenClient, data.RefreshToken, data.AccessToken))
                        using (var userInfoClient = new UserInfoClient(discoveryResponse.UserInfoEndpoint, refreshTokenHandler))
                        {
                            var response = await userInfoClient.GetAsync(data.AccessToken).ConfigureAwait(false);
                            if (!response.IsError)
                            {
                                var claimsIdentity = new ClaimsIdentity(response.Claims, "idSvr", "name", "role");
                                context.Console.WriteLine($"Logged in as {claimsIdentity.Name}. Access Token: {data.AccessToken}");
                                return;
                            }
                        }
                    }
                }
            }

            var browser = new SystemBrowser();
            var options = new OidcClientOptions
            {
                Authority = this.Authority,
                ClientId = Consts.IdentityServer.Client.Id,
                RedirectUri = Consts.IdentityServer.Client.RedirectUri + $":{browser.Port}",
                Scope = Consts.IdentityServer.Client.Scope,
                FilterClaims = false,
                Browser = browser
            };

            var oidcClient = new OidcClient(options);
            var result = await oidcClient.LoginAsync(new LoginRequest()).ConfigureAwait(false);
            if (result.IsError)
            {
                context.Console.Error.WriteLine($"Error attempting to log in:{Environment.NewLine}{result.Error}");
                return;
            }

            context.Repository.SetCommandData(
                new CommandData
                {
                    Authority = this.Authority,
                    AccessToken = result.AccessToken,
                    RefreshToken = result.RefreshToken,
                    Service = this.Service,
                });

            context.Console.WriteLine($"Logged in as {result.User.Identity.Name}. Access Token: {result.AccessToken}");
        }

        public class Reset : ICommand
        {
            public Task ExecuteAsync(CommandContext context)
            {
                context.Repository.SetCommandData(null);
                return Task.CompletedTask;
            }
        }

#pragma warning disable CA1812
        private class Api
        {
            public string Title { get; set; }

            public string Version { get; set; }

            public string OS { get; set; }
        }
    }
}
