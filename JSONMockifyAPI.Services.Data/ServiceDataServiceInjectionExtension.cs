// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace JSONMockifyAPI.Services.Data
{
    using JSONMockifyAPI.Data.Repositories;
    using JSONMockifyAPI.Data.Repositories.Interfaces;
    using JSONMockifyAPI.Services.Data.Contracts;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceDataServiceInjectionExtension
    {
        public static IServiceCollection AddServiceDataDependecies(this IServiceCollection services)
        {
            services.AddRepositoryDependecies();
            services.AddTransient<IJSONMockService, JSONMockService>();
            return services;
        }
    }
}
