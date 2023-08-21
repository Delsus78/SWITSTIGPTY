using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace SWITSTIGPTY.Services.Repositories;

public abstract class MongoBaseModel
{
    [BsonId]
    public Int64 Id { get; set; }
    
    [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
    public DateTime DateCreation { get; set; }
    [BsonIgnore]
    [JsonIgnore]
    public Status Status { get; set; } = Status.None;
}

public enum Status
{
    ToDelete,
    ToUpdate,
    None
}