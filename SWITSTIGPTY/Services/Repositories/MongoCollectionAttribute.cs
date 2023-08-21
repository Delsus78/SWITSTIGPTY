namespace SWITSTIGPTY.Services.Repositories;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class MongoCollectionAttribute : System.Attribute
{
    public MongoCollectionAttribute(string collectionName)
    {
        CollectionName = collectionName;
    }

    public string CollectionName { get; }
}