using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace ArghyaC.Infrastructure.Mongo
{
    public class MongoReader //TODO: clean up, change names etc.
    {
        protected static IMongoClient _client;
        protected static IMongoDatabase _database;

        public MongoReader(string dbName = "test", string connectionString = null)
        {
            _client = connectionString == null ?  new MongoClient() : new MongoClient(connectionString);
            _database = _client.GetDatabase(dbName);
        }

        public List<BsonDocument> Get<TIn>(string dottedField, TIn value)
        {
            var collection = _database.GetCollection<BsonDocument>("restaurants");

            var filter = Builders<BsonDocument>.Filter.Eq(dottedField, value);
            var found = collection.Find(filter).ToList();
            return found;
        }

        public string GetRestaurants(string name)
        {
            var collection = _database.GetCollection<BsonDocument>("restaurants");

            var filter = Builders<BsonDocument>.Filter.Eq("name", name);
            var found = collection.Find(filter).ToList();
            return found.Count.ToString();
        }

        public void UpdateRestaurant()
        {
            var collection = _database.GetCollection<BsonDocument>("restaurants");
            var filter = Builders<BsonDocument>.Filter.Eq("name", "Juni");
            var update = Builders<BsonDocument>.Update
                .Set("cuisine", "American (New)")
                .CurrentDate("lastModified");
            var result = collection.UpdateOne(filter, update); //await UpdateOneAsync
        }

        public void UpdateRestaurants()
        {
            var collection = _database.GetCollection<BsonDocument>("restaurants");
            var filter = Builders<BsonDocument>.Filter.Eq("restaurant_id", "41156888");
            var update = Builders<BsonDocument>.Update.Set("address.street", "East 31st Street");
            var result = collection.UpdateMany(filter, update); //await UpdateOneAsync
        }

        public void ReplaceRestaurant()
        {
            var collection = _database.GetCollection<BsonDocument>("restaurants");
            var filter = Builders<BsonDocument>.Filter.Eq("restaurant_id", "41156888");
            var newDocument = new BsonDocument { { "name", "newName" }, { "structure", "does not matter" }, { "the_id", "to omit or be same" } };
            var result = collection.ReplaceOne(filter, newDocument);
        }

        public void DropRestaurants()
        {
            _database.DropCollection("collectionNameToDrop");
        }

        public void CreateSimpleIndex()
        {
            var collection = _database.GetCollection<BsonDocument>("restaurants");
            var keys = Builders<BsonDocument>.IndexKeys.Ascending("cuisine");
            collection.Indexes.CreateOne(keys);
        }

        public void CreateCompoundIndex()
        {
            var collection = _database.GetCollection<BsonDocument>("restaurants");
            var keys = Builders<BsonDocument>.IndexKeys.Ascending("cuisine").Ascending("address.zipcode");
            collection.Indexes.CreateOne(keys);
        }

        public bool InsertRestaurant(string name)
        {
            var document = new BsonDocument
            {
                { "address" , new BsonDocument
                    {
                        { "street", "2 Avenue" },
                        { "zipcode", "10075" },
                        { "building", "1480" },
                        { "coord", new BsonArray { 73.9557413, 40.7720266 } }
                    }
                },
                { "borough", "Manhattan" },
                { "cuisine", "Italian" },
                { "grades", new BsonArray
                    {
                        new BsonDocument
                        {
                            { "date", new DateTime(2014, 10, 1, 0, 0, 0, DateTimeKind.Utc) },
                            { "grade", "A" },
                            { "score", 11 }
                        },
                        new BsonDocument
                        {
                            { "date", new DateTime(2014, 1, 6, 0, 0, 0, DateTimeKind.Utc) },
                            { "grade", "B" },
                            { "score", 17 }
                        }
                    }
                },
                { "name", name },
                { "restaurant_id", "41704620" }
            };

            try
            {
                var collection = _database.GetCollection<BsonDocument>("restaurants");
                collection.InsertOne(document); // .InsertOneAsync(document);
                //collection.InsertOne();
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }
    }
}
