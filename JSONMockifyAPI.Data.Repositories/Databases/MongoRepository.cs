// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace JSONMockifyAPI.Data.Repositories.Databases
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using JSONMockifyAPI.Data.Models;
    using JSONMockifyAPI.Data.Repositories.Interfaces;
    using MongoDB.Driver;

    public class MongoRepository<TEntity> : IDBRepository<TEntity>
        where TEntity : BaseModel
    {
        private IMongoDatabase db;
        private IMongoCollection<TEntity> collection;
        private IMongoClient client;

        public MongoRepository()
        {
            this.client = new MongoClient();
            this.Db = this.client.GetDatabase("demoDb");
            this.Collection = this.Db.GetCollection<TEntity>(typeof(TEntity).Name);
        }

        protected IMongoDatabase Db { get => this.db; set => this.db = value; }

        protected IMongoCollection<TEntity> Collection { get => this.collection; set => this.collection = value; }

        public void Delete(TEntity entity)
        {
            this.Collection.DeleteOne(e => e.ID == entity.ID);
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
            return this.Collection
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
            entity.CreatedTimestamp = default(DateTimeOffset);
            this.Collection.InsertOne(entity);
            return entity;
        }

        public TEntity Update(TEntity entity)
        {
            TEntity originalEntity = this.Get(entity.ID);
            entity._id = originalEntity._id;
            entity.UpdatedTimestamp = default(DateTimeOffset);
            this.Collection.ReplaceOne(e => e.ID == entity.ID, entity);
            return entity;
        }

        public bool RecordExists(Guid id)
        {
            return this.Collection.Find<TEntity>(e => e.ID == id).Any();
        }
    }
}
