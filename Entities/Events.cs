namespace SimpleWebAppReact.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
/// <summary>
/// Class structure matches 1-1 with Building Table in database
/// </summary>
public class Events
{
    [BsonId]
    [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("eventName"), BsonRepresentation(BsonType.String)]
    public string? EventName { get; set; }
    
    [BsonElement("summary"), BsonRepresentation(BsonType.String)]
    public string? Summary { get; set; }

    [BsonElement("content"), BsonRepresentation(BsonType.String)]
    public string? Content { get; set; }

    [BsonElement("userID"), BsonRepresentation(BsonType.String)]
    public string? UserID { get; set; }

    [BsonElement("date"), BsonRepresentation(BsonType.DateTime)]
    public DateTime? Date { get; set; }

    [BsonElement("address"), BsonRepresentation(BsonType.String)]
    public string? Address { get; set; }
}
