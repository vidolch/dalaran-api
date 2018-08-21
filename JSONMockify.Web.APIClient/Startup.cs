// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace JSONMockify.Web.APIClient
{
    using Ironclad.FilterAttributes;
    using JSONMockifyAPI.Data.Models;
    using JSONMockifyAPI.Data.Mongo;
    using JSONMockifyAPI.Data.Repositories;
    using JSONMockifyAPI.Data.Repositories.Interfaces;
    using JSONMockifyAPI.Services.Data;
    using JSONMockifyAPI.Services.Data.Contracts;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(
                    "UI",
                    builder => builder
                    .WithOrigins("http://localhost:4200")
                    .AllowAnyHeader()
                    .AllowAnyMethod());
            });

            var mongoConnectionString = this.Configuration.GetConnectionString("mongo");
            var mongoUrl = new MongoDB.Driver.MongoUrl(mongoConnectionString);

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

            services.AddIdentityWithMongoStores(mongoConnectionString)
                .AddDefaultTokenProviders();

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
                .AddDataAnnotations();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors("UI");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
