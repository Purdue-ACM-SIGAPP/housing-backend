using MongoDB.Bson.Serialization.Attributes;
    using MongoDB.Bson;
namespace SimpleWebAppReact.Entities;
public class RoomType{
    [BsonId]
    [BsonElement("id"), BsonRepresentation(BsonType.String)]
    private string? id {get; set;}
    [BsonElement("peoplePerBedroom"), BsonRepresentation(BsonType.int)]
    private int? peoplePerBedroom {get; set;}
    [BsonElement("cost"), BsonRepresentation(BsonType.Double)]
    private double? cost {get; set;}
    [BsonElement("buildingId"), BsonRepresentation(BsonType.int)]
    private int? buildingId {get; set;}
    [BsonElement("description"), BsonRepresentation(BsonType.String)]
    private string? description {get; set;} 
    [BsonElement("images"), BsonRepresentation(BsonType.ObjectId)]
    private string[] images {get; set;}
}