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
    
    [BsonElement("address"), BsonRepresentation(BsonType.String)]
    public string? Address { get; set; }

    [BsonElement("type"), BsonRepresentation(BsonType.Int32)]
    public int? Type { get; set; }

    [BsonElement("average_star_rating"), BsonRepresentation(BsonType.Decimal128)]
    public decimal? AverageStarRating { get; set; }

    [BsonElement("number_of_ratings"), BsonRepresentation(BsonType.Int32)]
    public int? NumberOfRatings { get; set; }

    [BsonElement("description"), BsonRepresentation(BsonType.String)]
    public string? Descripiton { get; set; }

    [BsonElement("link_to_external"), BsonRepresentation(BsonType.String)]
    public string? LinkToExternal { get; set; }
}
