using JSONMockifyAPI.Data.Models;
using JSONMockifyAPI.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace JSONMockifyAPI.Data.Repositories.Databases
{
    public abstract class Repository<TEntity> : IRepository<TEntity>
        where TEntity : BaseModel
    {
        private IDBRepository<TEntity> _dbRepository;

        public Repository(IDBRepository<TEntity> dbRepository)
        {
            _dbRepository = dbRepository;
        }

        public void Delete(TEntity entity)
        {
            _dbRepository.Delete(entity);
        }

        public TEntity Get(Guid id)
        {
            return _dbRepository.Get(id);
        }

        public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate, int page, int size)
        {
            return _dbRepository.GetAll(predicate, page, size);
        }

        public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbRepository.GetAll(predicate);
        }

        public IQueryable<TEntity> GetAll()
        {
            return _dbRepository.GetAll();
        }

        public TEntity Insert(TEntity entity)
        {
            return _dbRepository.Insert(entity);
        }

        public bool RecordExists(Guid id)
        {
            return _dbRepository.RecordExists(id);
        }

        public TEntity Update(TEntity entity)
        {
            return _dbRepository.Update(entity);
        }
    }
}
