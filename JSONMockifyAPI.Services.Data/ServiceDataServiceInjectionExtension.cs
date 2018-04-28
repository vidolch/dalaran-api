using JSONMockifyAPI.Data.Repositories;
using JSONMockifyAPI.Data.Repositories.Interfaces;
using JSONMockifyAPI.Services.Data.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace JSONMockifyAPI.Services.Data
{
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
