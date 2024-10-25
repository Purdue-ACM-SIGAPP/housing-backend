namespace SimpleWebAppReact.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
/// <summary>
/// Class structure matches 1-1 with Building Table in database
/// </summary>
public class Review
{
    // static fields storing definitions of max/min rating values
    public const short MIN_RATING = 1;
    public const short MAX_RATING = 10;

    // database elements
    [BsonId]
    [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    
    [BsonElement("userId"), BsonRepresentation(BsonType.String)]
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
    
    [BsonElement("buildingId"), BsonRepresentation(BsonType.String)]
    public string? BuildingId { get; set; }
}
