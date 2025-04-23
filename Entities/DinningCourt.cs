namespace SimpleWebAppReact.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
/// <summary>
/// Class structure matches 1-1 with the Building table in the database
/// </summary>

public class DinningCourt : Building
{
    public DinningCourt()
    {
        BuildingType = "D";
    }
    // Food options always available at the court (e.g., salad bar, la fonda, deli bar)
    [BsonElement("stableOptions"), BsonRepresentation(BsonType.Array)]
    public List<string> StableOptions { get; set; }

    
    // Whether or not meal swipes are accepted
    [BsonElement("acceptsSwipes"), BsonRepresentation(BsonType.Boolean)]
    public bool? AcceptsSwipes { get; set; }
    
    // An array of hours when the location is busiest
    [BsonElement("busyHours"), BsonRepresentation(BsonType.Array)]
    public List<string> BusyHours { get; set; } = new();
    
    // Whether or not dining dollars are accepted
    [BsonElement("acceptsDiningDollars"), BsonRepresentation(BsonType.Boolean)]
    public bool? AcceptsDiningDollars { get; set; }
    
    // Whether or not BoilerExpress is accepted
    [BsonElement("acceptsBoilerExpress"), BsonRepresentation(BsonType.Boolean)]
    public bool? AcceptsBoilerExpress { get; set; }
    
}
