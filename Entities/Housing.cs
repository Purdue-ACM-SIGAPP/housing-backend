namespace SimpleWebAppReact.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

/// <summary>
/// Represents a housing building with specific attributes.
/// </summary>
public class Housing : Building
{
    // List of room objects, each storing attributes such as cost, capacity, and features
    [BsonElement("rooms")]
    public List<Room> Rooms { get; set; } = new List<Room>();

    [BsonElement("pianoNum"), BsonRepresentation(BsonType.Int32)]
    public int PianoNum { get; set; }

    [BsonElement("kitchenNum"), BsonRepresentation(BsonType.Int32)]
    public int KitchenNum { get; set; }

    [BsonElement("haveDinningCourt"), BsonRepresentation(BsonType.Boolean)]
    public bool HaveDinningCourt { get; set; }

    [BsonElement("haveBoilerMarket"), BsonRepresentation(BsonType.Boolean)]
    public bool HaveBoilerMarket { get; set; }

    [BsonElement("studySpaceNum"), BsonRepresentation(BsonType.Int32)]
    public int StudySpaceNum { get; set; }
    
}
