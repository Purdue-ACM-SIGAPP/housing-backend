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
    public string? userId { get; set; }
    
    [BsonElement("description"), BsonRepresentation(BsonType.String)]
    public string? description { get; set; }

    [BsonElement("createdAt"), BsonRepresentation(BsonType.DateTime)]
    public DateTime? createdAt { get; set; } = DateTime.Now;

    [BsonElement("rating"), BsonRepresentation(BsonType.Int32)]
    public int? rating { get; set; }

    [BsonElement("likeCount"), BsonRepresentation(BsonType.Int32)]
    public int? likeCount { get; set; }

    [BsonElement("dislikeCount"), BsonRepresentation(BsonType.Int32)]
    public int? dislikeCount { get; set; }
}
