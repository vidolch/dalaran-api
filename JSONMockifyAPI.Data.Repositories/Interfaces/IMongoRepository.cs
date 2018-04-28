using JSONMockifyAPI.Data.Models;

namespace JSONMockifyAPI.Data.Repositories.Interfaces
{
    public interface IMongoRepository<TEntity> : IRepository<TEntity>
        where TEntity : BaseModel
    {
    }
}
