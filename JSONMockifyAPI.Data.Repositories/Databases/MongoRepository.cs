using JSONMockifyAPI.Data.Models;
using JSONMockifyAPI.Data.Repositories.Interfaces;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace JSONMockifyAPI.Data.Repositories.Databases
{
    public class MongoRepository<TEntity> : IDBRepository<TEntity>
        where TEntity : BaseModel
    {
        protected IMongoDatabase db;
        protected IMongoCollection<TEntity> collection;
        private IMongoClient _client;

        public MongoRepository()
        {
            _client = new MongoClient();
            db = _client.GetDatabase("demoDb");
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

        public TEntity Get(Guid id)
        {
            return this.GetAll()
                .FirstOrDefault(e => e.ID == id);
        }

        public TEntity Insert(TEntity entity)
        {
            entity.ID = Guid.NewGuid();
            this.collection.InsertOne(entity);
            return entity;
        }

        public TEntity Update(TEntity entity)
        {
            TEntity originalEntity = this.Get(entity.ID);
            entity._id = originalEntity._id;
            this.collection.ReplaceOne(e => e.ID == entity.ID, entity);
            return entity;
        }

        public bool RecordExists(Guid id)
        {
            return this.collection.Find<TEntity>(e => e.ID == id).Any();
        }
    }
}
