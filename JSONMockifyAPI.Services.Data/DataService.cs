using System;
using System.Collections.Generic;
using JSONMockifyAPI.Data.Models;
using JSONMockifyAPI.Data.Repositories.Interfaces;
using JSONMockifyAPI.Services.Data.Contracts;

namespace JSONMockifyAPI.Services.Data
{
    public class DataService<TEntity> : IDataService<TEntity>
        where TEntity : BaseModel
    {
        private IRepository<TEntity> _repository;

        public DataService(IRepository<TEntity> repository)
        {
            _repository = repository;
        }

        public TEntity Create(TEntity entity)
        {
            return _repository.Insert(entity);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return _repository.GetAll();
        }

        public TEntity Get(Guid id)
        {
            return _repository.Get(id);
        }

        public void Delete(TEntity entity)
        {
            _repository.Delete(entity);
        }

        public void Delete(Guid id)
        {
            TEntity entity = this.Get(id);
            if (entity != null)
            {
                _repository.Delete(entity);
            }
        }

        public TEntity Update(TEntity entity)
        {
            return _repository.Update(entity);
        }

        public bool RecordExists(Guid id)
        {
            return _repository.RecordExists(id);
        }
    }
}
