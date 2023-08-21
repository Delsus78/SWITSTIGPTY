using MongoDB.Driver;

namespace SWITSTIGPTY.Services.Repositories;

public class MongoDbRepository
{
    public IMongoDatabase Database { get; }

    public MongoDbRepository(IMongoClient client, string dbName)
    {
        Database = client.GetDatabase(dbName);
    }

    public IMongoCollection<T> GetCollection<T>(ReadPreference readPreference = null) where T : MongoBaseModel
    {
        return Database
            .WithReadPreference(readPreference ?? ReadPreference.Primary)
            .GetCollection<T>(GetCollectionName<T>());
    }

    public static string GetCollectionName<T>() where T : class
    {
        return (typeof(T).GetCustomAttributes(typeof(MongoCollectionAttribute), true).FirstOrDefault() as MongoCollectionAttribute)?.CollectionName;
    }
        
}