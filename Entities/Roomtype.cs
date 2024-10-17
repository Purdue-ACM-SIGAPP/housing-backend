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

    [BsonElement("peoplePerBedroom"), BsonRepresentation(BsonType.Int32)]
    public int? PeoplePerBedroom { get; set; }

    [BsonElement("cost"), BsonRepresentation(BsonType.String)]
    public string? Cost { get; set; }

    [BsonElement("buildingId"), BsonRepresentation(BsonType.ObjectId)]
    public ObjectId? BuildingID { get; set; }

    [BsonElement("description"), BsonRepresentation(BsonType.String)]
    public string? Description { get; set; }

    [BsonElement("images"), BsonRepresentation(BsonType.String)]
    public string[]? Images { get; set; }

}
