// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace Dalaran.Web.APIClient
{
    using System.Runtime.InteropServices;
    using Dalaran.Core.Domain;
    using Dalaran.Core.Express;
    using Dalaran.Data.Models;
    using Dalaran.Data.Mongo;
    using Dalaran.Data.Repositories;
    using Dalaran.Data.Repositories.Interfaces;
    using Dalaran.Services.Data;
    using Dalaran.Services.Data.Contracts;
    using Dalaran.Web.APIClient.Database;
    using Dalaran.Web.APIClient.Middlewares;
    using IdentityServer4.AccessTokenValidation;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;
    using Serilog;

    public class Startup
    {
        private readonly Settings settings;

        public Startup(IConfiguration configuration)
        {
            this.settings = configuration.Get<Settings>(options => options.BindNonPublicProperties = true);
            this.settings.Validate();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvcCore(options =>
                {
                    options.Filters.Add(new RequestLogAttribute());
                })
                .AddJsonFormatters(serializerSettings =>
                {
                    serializerSettings.ContractResolver = new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() };
                    serializerSettings.Converters.Add(new StringEnumConverter());
                    serializerSettings.NullValueHandling = NullValueHandling.Ignore;
                })
                .AddAuthorization()
                .AddDataAnnotations();

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(
                    options =>
                    {
                        options.Authority = this.settings.Auth.Authority;
                        options.ApiName = this.settings.Auth.ClientId;
                        options.ApiSecret = this.settings.Auth.Secret;

                        options.RequireHttpsMetadata = false;
                    });

            Log.Information($"Auth server: {this.settings.Auth.Authority}");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Log.Information("Platform is Windows");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Log.Information("Platform is Linux");
            }

            services.AddCors(options =>
            {
                options.AddPolicy("spa", policy =>
                {
                    policy.WithOrigins(this.settings.Auth.AllowedCors)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            var mongoUrl = new MongoDB.Driver.MongoUrl(this.settings.Mongo.ConnectionString);

            services.AddSingleton<IDBRepository<Request>>(
                new MongoRepository<Request>(mongoUrl, nameof(Request)));
            services.AddSingleton<IDBRepository<Resource>>(
                new MongoRepository<Resource>(mongoUrl, nameof(Resource)));
            services.AddSingleton<IDBRepository<Collection>>(
                new MongoRepository<Collection>(mongoUrl, nameof(Collection)));

            services.AddTransient<IRequestRepository, RequestRepository>();
            services.AddTransient<IResourceRepository, ResourceRepository>();
            services.AddTransient<ICollectionRepository, CollectionRepository>();

            services.AddTransient<IRequestService, RequestService>();
            services.AddTransient<IResourceService, ResourceService>();
            services.AddTransient<ICollectionService, CollectionService>();

            services.AddSingleton<IApiGenerator>(o =>
            {
                return new ExpressApiGenerator(this.settings.Template.Path);
            });

            services.AddSingleton<Seeder>();

            var seeder = services.BuildServiceProvider().GetService<Seeder>();
            seeder.Seed().GetAwaiter();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors("spa");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
