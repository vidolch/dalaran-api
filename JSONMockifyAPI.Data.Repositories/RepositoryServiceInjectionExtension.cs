using JSONMockifyAPI.Data.Models;
using JSONMockifyAPI.Data.Repositories.Databases;
using JSONMockifyAPI.Data.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;

namespace JSONMockifyAPI.Data.Repositories
{
    public static class RepositoryServiceInjectionExtension
    {
        public static IServiceCollection AddRepositoryDependecies(this IServiceCollection services)
        {
            BsonClassMap.RegisterClassMap<BaseModel>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapProperty(x => x.ID).SetElementName("GID");
            });
            services.AddTransient(typeof(IDBRepository<>), typeof(MongoRepository<>));
            services.AddTransient<IJSONMockRepository, JSONMockRepository>();
            return services;
        }
    }
}
