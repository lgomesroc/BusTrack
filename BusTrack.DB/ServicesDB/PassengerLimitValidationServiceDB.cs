using MongoDB.Bson;
using MongoDB.Driver;

namespace BusTrack.BusTrack.DB.ServicrDB
{
    public class PassengerLimitValidationServiceDB
    {
        public void ConfigurePassengerLimitValidation()
        {
            var validator = new BsonDocument
            {
                {
                    "validator", new BsonDocument
                    {
                        {
                            "$jsonSchema", new BsonDocument
                            {
                                {"bsonType", "object"},
                                {
                                    "required", new BsonArray {"passengers"}
                                },
                                {
                                    "properties", new BsonDocument
                                    {
                                        {
                                            "passengers", new BsonDocument
                                            {
                                                {"bsonType", "array"},
                                                {"maxItems", 50},
                                                {"errorMessage", "Número máximo de passageiros excedido (limite: 50)"}
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var collection = new MongoClient().GetDatabase("BusTrack").GetCollection<BsonDocument>("Trips");
            var options = new CreateCollectionOptions<BsonDocument>
            {
                Validator = validator
            };

            collection.Database.CreateCollection("Trips", options);
        }

        public static void ApplyPassengerLimitValidation(IMongoDatabase database)
        {
            var tripsCollection = database.GetCollection<BsonDocument>("Trips");

            var validator = new BsonDocument
    {
        {
            "$jsonSchema", new BsonDocument
            {
                { "bsonType", "object" },
                { "required", new BsonArray { "passengers" } },
                { "properties", new BsonDocument
                    {
                        { "passengers", new BsonDocument
                            {
                                { "bsonType", "array" },
                                { "maxItems", 50 },
                                { "errorMessage", "Número máximo de passageiros excedido (limite: 50)" }
                            }
                        }
                    }
                }
            }
        }
    };

            var validationOptions = new CreateCollectionOptions<BsonDocument>
            {
                Validator = validator
            };

            if (!CollectionExists(database, "Trips"))
            {
                database.CreateCollection("Trips", validationOptions);
            }
            else
            {
                var updateOptions = new UpdateOptions { IsUpsert = true };
                var filter = new BsonDocument();
                var update = new BsonDocument("$set", new BsonDocument("validator", validator));
                database.RunCommand<BsonDocument>(new BsonDocumentCommand<BsonDocument>(update));
            }
        }

        private static bool CollectionExists(IMongoDatabase database, string collectionName)
        {
            var filter = new BsonDocument("name", collectionName);
            var collections = database.ListCollections(new ListCollectionsOptions { Filter = filter });
            return collections.Any();
        }
    }
}

