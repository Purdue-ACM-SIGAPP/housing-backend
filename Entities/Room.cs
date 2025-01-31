using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
public class Room
{
    [BsonElement("capacity"), BsonRepresentation(BsonType.Int32)]
    public int Capacity { get; set; }

    [BsonElement("features")]
    public List<string> Features { get; set; } = new List<string>();

    [BsonElement("cost"), BsonRepresentation(BsonType.Decimal128)]
    public decimal Cost { get; set; }

    public double HousingRate {get; set; }

    public bool IsSharedBathroom {get; set; }

    [BsonElement("buildingId"), BsonRepresentation(BsonType.ObjectId)]

    public string? BuildingID { get; set; }

}