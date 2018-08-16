// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace JSONMockifyAPI.Data.Mongo
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Security.Authentication;
    using System.Threading.Tasks;
    using JSONMockifyAPI.Data.Models;
    using JSONMockifyAPI.Data.Repositories.Interfaces;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using MongoDB.Driver;

    public class MongoRepository<TIdentity, TEntity> : IDBRepository<TIdentity, TEntity>
        where TIdentity : class
        where TEntity : BaseModel
    {
        private static readonly FindOneAndReplaceOptions<Document> UpdateOptions = new FindOneAndReplaceOptions<Document> { IsUpsert = true };

        private readonly IMongoClient client;
        private IMongoDatabase db;
        private IMongoCollection<Document> collection;

        public MongoRepository(MongoUrl mongoUrl, string schema)
        {
            MongoClientSettings settings = MongoClientSettings.FromUrl(mongoUrl);

            if (mongoUrl.UseSsl)
            {
                settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
            }

            this.client = new MongoClient();
            this.Db = this.client.GetDatabase(mongoUrl.DatabaseName);
            this.Collection = this.Db.GetCollection<Document>(string.Join(".", schema, typeof(TEntity).Name));
        }

        protected IMongoDatabase Db { get => this.db; set => this.db = value; }

        private IMongoCollection<Document> Collection { get => this.collection; set => this.collection = value; }

        public async Task<(IEnumerable<TEntity>, long)> GetAllAsync(Expression<Func<TEntity, bool>> predicate = default, int page = 1, int size = 20)
        {
            page = page < 1 ? 1 : page;
            var filter = predicate == null ? _ => true : Filter.Convert(predicate);
            var matches = this.Collection.Find(filter);
            var count = await matches.CountDocumentsAsync();
            var result = await matches.Skip(size * (page - 1)).Limit(size).ToListAsync().ConfigureAwait(false);
            return (result.Select(document => document.Entity), count);
        }

        public async Task<TEntity> GetAsync(TIdentity identity)
        {
            var filter = Builders<Document>.Filter.Eq(x => x.Identity, identity);
            var document = await this.Collection.Find(filter).FirstOrDefaultAsync().ConfigureAwait(false);
            return document?.Entity;
        }

        public async Task AddOrUpdateAsync(TIdentity identity, TEntity entity)
        {
            var document = new Document() { Identity = identity, Entity = entity };
            var filter = Builders<Document>.Filter.Eq(x => x.Identity, identity);
            await this.collection.FindOneAndReplaceAsync(filter, document, UpdateOptions).ConfigureAwait(false);
        }

        public async Task<bool> DeleteAsync(TIdentity identity)
        {
            var filter = Builders<Document>.Filter.Eq(x => x.Identity, identity);
            var document = await this.collection.FindOneAndDeleteAsync(filter).ConfigureAwait(false);
            return document != null;
        }

        public async Task<bool> RecordExistsAsync(TIdentity identity)
        {
            var filter = Builders<Document>.Filter.Eq(x => x.Identity, identity);
            var document = await this.collection.FindAsync(filter).ConfigureAwait(false);
            return document.Any();
        }

        private class Filter : ExpressionVisitor
        {
            private ReadOnlyCollection<ParameterExpression> parameters;

            public static Expression<Func<Document, bool>> Convert<T>(Expression<T> root)
            {
                var visitor = new Filter();
                var expression = (Expression<Func<Document, bool>>)visitor.Visit(root);
                return expression;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return (this.parameters != null)
                    ? this.parameters.FirstOrDefault(p => p.Name == node.Name)
                    : node.Type == typeof(TEntity)
                        ? Expression.Parameter(typeof(Document), node.Name)
                        : node;
            }

            protected override Expression VisitLambda<T>(Expression<T> node)
            {
                this.parameters = this.VisitAndConvert<ParameterExpression>(node.Parameters, "VisitLambda");
                return Expression.Lambda(this.Visit(node.Body), this.parameters);
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                if (node.Member.DeclaringType == typeof(TEntity))
                {
                    var expression = this.Visit(node.Expression);
                    var member = typeof(Document).GetProperty("Entity");
                    var entityExpression = Expression.MakeMemberAccess(expression, member);
                    var otherMember = typeof(TEntity).GetProperty(node.Member.Name);
                    return Expression.MakeMemberAccess(entityExpression, otherMember);
                }

                return base.VisitMember(node);
            }
        }

        private sealed class Document
        {
            [BsonId]
            [BsonRepresentation(BsonType.String)]
            public TIdentity Identity { get; set; }

            public TEntity Entity { get; set; }
        }
    }
}
