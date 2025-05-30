namespace SimpleWebAppReact.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
/// <summary>
/// 
/// </summary>
public class VideoTour
{
    [BsonId]
    [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    
    [BsonElement("title"), BsonRepresentation(BsonType.String)]
    public string? Title { get; set; }
    
    [BsonElement("url"), BsonRepresentation(BsonType.String)]
    public string? Url { get; set; }

    [BsonElement("length"), BsonRepresentation(BsonType.Double)]
    public Double? Length { get; set; }

    [BsonElement("createdAt"), BsonRepresentation(BsonType.DateTime)]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [BsonElement("description"), BsonRepresentation(BsonType.String)]
    public string? Description { get; set; }

    [BsonElement("userId"), BsonRepresentation(BsonType.String)]
    public string? UserId { get; set; }

    [BsonElement("buildingId"), BsonRepresentation(BsonType.String)]
    public string? BuildingId { get; set; }
    
    [BsonElement("image"), BsonRepresentation(BsonType.String)]
    public string? Image { get; set; }
}
