// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace JSONMockify.Web.APIClient
{
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

            services.AddSingleton<IDBRepository<string, JSONMock>>(
                new MongoRepository<string, JSONMock>(mongoUrl, nameof(JSONMock)));
            services.AddTransient<IJSONMockRepository, JSONMockRepository>();
            services.AddTransient<IJSONMockService, JSONMockService>();
            services.AddIdentityWithMongoStores(mongoConnectionString)
                .AddDefaultTokenProviders();

            services.AddMvc();
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
