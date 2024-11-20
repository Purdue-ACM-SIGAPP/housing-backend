using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
public class Room
{
    public string? Type { get; set; }  
    public int Capacity { get; set; } 

    public double HousingRate {get; set; }

    public bool IsSharedBathroom {get; set; }

    [BsonElement("buildingId"), BsonRepresentation(BsonType.ObjectId)]

    public string? BuildingID { get; set; }

}