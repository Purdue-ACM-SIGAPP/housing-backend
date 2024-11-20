using AspNetCore.Identity.Mongo.Model;

namespace SimpleWebAppReact.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
/// <summary>
/// Class structure matches 1-1 with User Table in database
/// </summary>
public class User : MongoUser
{
    [BsonId]
    [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    
    [BsonElement("name"), BsonRepresentation(BsonType.String)]
    public string? Name { get; set; }
    
    [BsonElement("phoneNumber"), BsonRepresentation(BsonType.String)]
    public string? PhoneNumber { get; set; }

    [BsonElement("accountType"), BsonRepresentation(BsonType.Int32)]
    public int? AccountType { get; set; }
}
