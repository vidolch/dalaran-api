using JSONMockifyAPI.Data.Models;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace JSONMockifyAPI.Data.Repositories.Interfaces
{
    public interface IRepository<IEntity>
        where IEntity : BaseModel
    {
        void Insert(IEntity entity);

        void Update(IEntity entity);

        void Delete(IEntity entity);

        IQueryable<IEntity> GetAll(Expression<Func<IEntity, bool>> predicate, int page, int size);

        IQueryable<IEntity> GetAll(Expression<Func<IEntity, bool>> predicate);

        IQueryable<IEntity> GetAll();

        IEntity GetById(Guid id);
    }
}
