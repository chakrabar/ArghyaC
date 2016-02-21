using ArghyaC.Infrastructure.Utilities;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ArghyaC.Infrastructure.Mongo
{
    public class MongoRepository
    {
        readonly IMongoClient _client;
        readonly IMongoDatabase _database;
        readonly string _collectionName;

        public MongoRepository(string collectionName)
            : this(ConfigUtilities.GetAppSettings("MongoDatabase"), collectionName, ConfigUtilities.GetAppSettings("MongoConnection"))
        { }

        public MongoRepository(string dbName, string collectionName, string connectionString = null)
        {
            _client = connectionString == null ?  new MongoClient() : new MongoClient(connectionString);
            _database = _client.GetDatabase(dbName);
            _collectionName = collectionName;
        }

        public bool CreateSimpleCollection(string collectionName)
        {
            try
            {
                _database.CreateCollection(collectionName);
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public IQueryable<T> Get<T>()
        {
            var collection = _database.GetCollection<T>(_collectionName);
            return collection.AsQueryable<T>();
        }

        public bool Insert<T>(T data)
        {
            try
            {
                var collection = _database.GetCollection<T>(_collectionName);
                collection.InsertOne(data);
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public bool InsertMany<T>(IEnumerable<T> data)
        {
            try
            {
                var collection = _database.GetCollection<T>(_collectionName);
                collection.InsertMany(data);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(MongoEntity entity)
        {
            try
            {
                var collection = _database.GetCollection<MongoEntity>(_collectionName);
                //var k = Builders<BsonDocument>.Filter.Eq("_id", id);
                //var result = collection.ReplaceOne(
                //                        item => item.Id == id, 
                //                        myObject,
                //                        new UpdateOptions { IsUpsert = true });
                var result = collection.ReplaceOne(
                                        item => item.Id == entity.Id,
                                        entity,
                                        new UpdateOptions { IsUpsert = true });
                return result.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DeleteById(ObjectId id)
        {
            try
            {
                var collection = _database.GetCollection<BsonDocument>(_collectionName);
                var k = Builders<BsonDocument>.Filter.Eq("_id", id);
                var result = collection.DeleteOne(k);
                return result.DeletedCount > 0; //TODO: check
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete<TEntity>(Func<TEntity, bool> selector) where TEntity : MongoEntity
        {
            try
            {
                var foundFromDb = Get<TEntity>().FirstOrDefault(selector);
                if (foundFromDb != null)
                {
                    return DeleteById(foundFromDb.Id);
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //public bool Delete<TEntity, TProperty>(string collectionName, Func<TEntity, TProperty> selector, TProperty propValue) where TProperty : IEquatable<TProperty>
        //{
        //    try
        //    {
        //        var collection = _database.GetCollection<TEntity>(collectionName);

        //        Expression<Func<TEntity, bool>> expressionForTree = t => selector(t).Equals(propValue);

        //        //var q = collection.Find<TEntity>(selector);
        //        var result = collection.DeleteOne(expressionForTree);
        //        return result.DeletedCount > 0; //TODO: check
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}

        //public bool Update<TEntity, TProperty>(TEntity data, string collectionName, Func<TEntity, TProperty> selector, TProperty propValue) where TProperty : IEquatable<TProperty>
        //{
        //    try
        //    {
        //        var collection = _database.GetCollection<TEntity>(_collectionName);

        //        Expression<Func<TEntity, bool>> expressionForTree = t => selector(t).Equals(propValue);

        //        //var q = collection.Find<TEntity>(selector);
        //        var result = collection.ReplaceOne(expressionForTree, data);
        //        return result.IsAcknowledged; //TODO: check
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}
    }
}
