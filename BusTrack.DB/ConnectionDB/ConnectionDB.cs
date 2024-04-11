﻿using MongoDB.Driver;

namespace BusTrack.BusTrack.DB.ConnectionDB
{
    public class ConnectionDB
    {
        public IMongoDatabase Connect(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            return client.GetDatabase(databaseName);
        }
    }
}
