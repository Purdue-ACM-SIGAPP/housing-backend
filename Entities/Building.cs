namespace SimpleWebAppReact.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
/// <summary>
/// Class structure matches 1-1 with Building Table in database
/// </summary>
public class Building
{
    [BsonId]
    [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    
    [BsonElement("name"), BsonRepresentation(BsonType.String)]
    public string? Name { get; set; }
    
    [BsonElement("acronym"), BsonRepresentation(BsonType.String)]
    public string? Acronym { get; set; }

    [BsonElement("address"), BsonRepresentation(BsonType.String)]
    public string? Address { get; set; }

    [BsonElement("latitude"), BsonRepresentation(BsonType.Double)]
    public double? Latitude { get; set; }
    
    [BsonElement("longitude"), BsonRepresentation(BsonType.Double)]
    public double? Longitude { get; set; }
    
    [BsonElement("image"), BsonRepresentation(BsonType.String)]
    public string? Image { get; set; }
}
