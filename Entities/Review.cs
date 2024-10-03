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
    public string? UserId { get; set; }
    
    [BsonElement("description"), BsonRepresentation(BsonType.String)]
    public string? Description { get; set; }

    [BsonElement("createdAt"), BsonRepresentation(BsonType.DateTime)]
    public DateTime? CreatedAt { get; set; } = DateTime.Now;

    [BsonElement("rating"), BsonRepresentation(BsonType.Int32)]
    public int? Rating { get; set; }

    [BsonElement("likeCount"), BsonRepresentation(BsonType.Int32)]
    public int? LikeCount { get; set; }

    [BsonElement("dislikeCount"), BsonRepresentation(BsonType.Int32)]
    public int? DislikeCount { get; set; }
}
