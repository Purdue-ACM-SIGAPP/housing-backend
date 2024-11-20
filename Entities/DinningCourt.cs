namespace SimpleWebAppReact.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
/// <summary>
/// Class structure matches 1-1 with Building Table in database
/// </summary>
public class DinningCourt
{
    // food options always available at the court (ex/ salad bar, la fonda, deli bar) 
    [BsonElement("stableOptions"), BsonRepresentation(BsonType.Array)]
    public Array? StableOptions { get; set; }
    
    // whether or not swipes are accepted
    [BsonElement("acceptsSwipes"), BsonRepresentation(BsonType.Boolean)]
    public bool? AcceptsSwipes { get; set; }
    
    // an array of hours when the location is busiest (strings?) 
    [BsonElement("busyHours"), BsonRepresentation(BsonType.Array)]
    public bool? BusyHours { get; set; }
    
    // whether or not dining dollars are accepted
    [BsonElement("acceptsDiningDollars"), BsonRepresentation(BsonType.Boolean)]
    public bool? AcceptsDiningDollars { get; set; }
    
    // whether or not boilerExpress is accepted
    [BsonElement("acceptsBoilerExpress"), BsonRepresentation(BsonType.Boolean)]
    public bool? AcceptsBoilerExpress { get; set; }
    
}