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

//     <field>: { type: <GeoJSON type> , coordinates: <coordinates> }



//     location: {
//         type: "Point",
//         coordinates: [-73.856077, 40.848447]
//     }
//     <field>: [<longitude>, <latitude> ]

}
