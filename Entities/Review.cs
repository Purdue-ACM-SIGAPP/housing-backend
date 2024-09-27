namespace SimpleWebAppReact.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
/// <summary>
/// Class structure matches 1-1 with Building Table in database
/// </summary>
public class Review
{
    [BsonId]
    [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    
    [BsonElement("creator"), BsonRepresentation(BsonType.String)]
    public string? userID { get; set; }
    
    [BsonElement("description"), BsonRepresentation(BsonType.String)]
    public string? description { get; set; }

    [BsonElement("createdAt"), BsonRepresentation(BsonType.DateTime)]
    public DateTime? createdAt { get; set; }

    [BsonElement("rating"), BsonRepresentation(BsonType.int)]
    public int? createdAt { get; set; }

    [BsonElement("likeCount"), BsonRepresentation(BsonType.int)]
    public int? likeCount { get; set; }

    [BsonElement("dislikeCount"), BsonRepresentation(BsonType.int)]
    public int? dislikeCount { get; set; }
}
