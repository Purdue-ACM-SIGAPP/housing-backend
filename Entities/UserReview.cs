namespace SimpleWebAppReact.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
/// <summary>
/// Class structure matches 1-1 with UserReview Table in database
/// </summary>
public class UserReview
{
    [BsonId]
    [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    
    [BsonElement("userId"), BsonRepresentation(BsonType.String)]
    public string? UserId { get; set; }
    
    [BsonElement("reviewId"), BsonRepresentation(BsonType.String)]
    public string? ReviewId { get; set; }
}