using JSONMockifyAPI.Data.Models;
using JSONMockifyAPI.Data.Repositories.Interfaces;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace JSONMockifyAPI.Data.Repositories
{
    public class MongoRepository<TEntity> : IRepository<TEntity>
        where TEntity : BaseModel
    {
        protected IMongoDatabase db;
        protected IMongoCollection<TEntity> collection;

        public MongoRepository(IMongoDatabase db)
        {
            this.db = db;
            this.collection = db.GetCollection<TEntity>(typeof(TEntity).Name);
        }

        public void Delete(TEntity entity)
        {
            this.collection.DeleteOne(e => e.ID == entity.ID);
        }

        public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate, int page, int size)
        {
            return this.GetAll(predicate)
                .Skip((page - 1) * size)
                .Take(size); 
        }

        public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate)
        {
            return this.GetAll()
                .Where(predicate);
        }

        public IQueryable<TEntity> GetAll()
        {
            return this.collection
                .AsQueryable<TEntity>();
        }

        public TEntity GetById(Guid id)
        {
            return this.GetAll()
                .FirstOrDefault(e => e.ID == id);
        }

        public void Insert(TEntity entity)
        {
            this.collection.InsertOne(entity);
        }

        public void Update(TEntity entity)
        {
            this.collection.ReplaceOne(e => e.ID == entity.ID, entity);
        }
    }
}
