using JSONMockifyAPI.Data.Models;
using System;
using System.Collections.Generic;

namespace JSONMockifyAPI.Services.Data.Contracts
{
    public interface IDataService<TEntity> where TEntity : BaseModel
    {
        IEnumerable<TEntity> GetAll();
        TEntity Create(TEntity entity);
        TEntity Get(Guid id);
        void Delete(TEntity entity);
        void Delete(Guid id);
        TEntity Update(TEntity entity);
        bool RecordExists(Guid id);
    }
}