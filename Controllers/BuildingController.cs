using FuzzySharp;
using Microsoft.AspNetCore.Mvc;
using SimpleWebAppReact.Entities;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using SimpleWebAppReact.Services;

namespace SimpleWebAppReact.Controllers
{

    /// <summary>
    /// Defines endpoints for operations relating the Building table
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class BuildingController : ControllerBase
    {
        private readonly ILogger<BuildingController> _logger;
        private readonly IMongoCollection<Building>? _buildings;
        private readonly BuildingOutlineService _buildingOutlineService;
        private readonly HttpClient _httpClient;
        private readonly string _googleApiKey = "AIzaSyCzKs4kUhXuPhBxYB2BU0ODXXIUBJnenhA";

        public BuildingController(ILogger<BuildingController> logger, MongoDbService mongoDbService,  BuildingOutlineService buildingOutlineService, HttpClient httpClient)
        {
            _logger = logger;
            _buildings = mongoDbService.Database?.GetCollection<Building>("building");
            _buildingOutlineService = buildingOutlineService;
            _httpClient = httpClient;
        }

        /// <summary>
        /// gets buildings using an approximate search
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<Building>> Get(
            [FromQuery] string? query = null,
            [FromQuery] int? pageLength = null,
            [FromQuery] int? pageIndex = null)
        {
            int FuzzScore(Building building)
            {
                return Math.Max(
                    building.Name == null ? 0 : Fuzz.Ratio(query, building.Name),
                    building.Acronym == null ? 0 : Fuzz.Ratio(query, building.Acronym)
                );
            }

            var filter = FilterDefinition<Building>.Empty;
            var buildings = await _buildings.Find(filter).ToListAsync();

            if (!string.IsNullOrEmpty(query))
            {
                buildings.Sort((b1, b2) =>
                {
                    var s1 = FuzzScore(b1);
                    var s2 = FuzzScore(b2);

                    // Sort descending
                    return -s1.CompareTo(s2);
                });
            }

            if (pageLength is > 0)
            {
                var index = (pageIndex ?? 0) * pageLength.Value;
                if (index >= 0 && index < buildings.Count)
                {
                    var count = Math.Min(pageLength.Value, buildings.Count - index);
                    buildings = buildings.GetRange(index, count);
                }
                else
                {
                    buildings = new List<Building>();
                }
            }

            return buildings;
        }

        /// <summary>
        /// gets specific building with id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Building?>> GetById(string id)
        {
            // Simple validation to check if the ID is not null
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Invalid ID format.");
            }

            var filter = Builders<Building>.Filter.Eq(x => x.Id, id);
            var building = _buildings.Find(filter).FirstOrDefault();
            return building is not null ? Ok(building) : NotFound();
        }

        /// <summary>
        /// adds building entry to table
        /// </summary>
        /// <param name="building"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Post(Building building)
        {
            // Prepare the Geocoding API request
            string url = $"https://maps.googleapis.com/maps/api/geocode/json?address={building.Address}&key={_googleApiKey}";

            try
            {
                // Send the request to Google Maps Geocoding API
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var jsonData = JObject.Parse(jsonResponse);

                // Parse coordinates from the response
                var location = jsonData["results"]?[0]?["geometry"]?["location"];
                var latitude = location?["lat"]?.ToObject<double>();
                var longitude = location?["lng"]?.ToObject<double>();

                if (latitude == null || longitude == null)
                {
                    return BadRequest(new { message = "Could not get coordinates from the address." });
                }

                // Set the coordinates in the building entity
                building.Latitude = latitude.Value;
                building.Longitude = longitude.Value;
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Error while calling Google Maps API");
                return StatusCode(500, new { message = "Error while calling Google Maps API." });
            }
            
            await _buildings.InsertOneAsync(building);
            return CreatedAtAction(nameof(GetById), new { id = building.Id }, building);
        }

        /// <summary>
        /// updates a building entry
        /// </summary>
        /// <param name="building"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<ActionResult> Update(Building building)
        {
            var filter = Builders<Building>.Filter.Eq(x => x.Id, building.Id);
            await _buildings.ReplaceOneAsync(filter, building);
            return Ok();
        }

        /// <summary>
        /// deletes a building entry
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var filter = Builders<Building>.Filter.Eq(x => x.Id, id);
            await _buildings.DeleteOneAsync(filter);
            return Ok();
        }

        /// <summary>
        /// Gets a building's outline, made of coordinates
        /// </summary>
        /// <param name="id"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        [HttpGet("outline")]
        public async Task<ActionResult> GetBuildingOutlineByPoint(
            [FromQuery] string id,
            [FromQuery] double radius = 0.001) // default radius ~100m
        {
            double targetLatitude, targetLongitude;

            // ID is given
            if (!string.IsNullOrEmpty(id))
            {
                // Query the database to get the building coordinates using the building's ID
                var filter = Builders<Building>.Filter.Eq(x => x.Id, id);
                var building = _buildings.Find(filter).FirstOrDefault();

                if (building == null)
                {
                    return NotFound($"Building with ID {id} not found.");
                }

                targetLatitude = building.Latitude ?? 0.0;
                targetLongitude = building.Longitude ?? 0.0;
            }
            else
            {
                // Return error if id not provided
                return BadRequest("Either id must be provided.");
            }

            // Retrieve building outlines within the bounding box
            var outlineTuples = await _buildingOutlineService.GetBuildingOutline(targetLatitude, targetLongitude, radius);

            // convert the returned variable into a object that can be serialized
            var buildingOutlines = outlineTuples.Select((outline, index) => new BuildingOutline
            {
                BuildingID = id ?? $"building_{index + 1}",
                Coordinates = outline.Select(coordinate => new Coordinate
                {
                    Latitude = coordinate.Lat,
                    Longitude = coordinate.Lon
                }).ToList()
            }).ToList();

            // Print each building outline's coordinates to the console
            // PrintBuildingOutlines(buildingOutlines);

            // Return the building outlines as JSON
            return Ok(buildingOutlines);
        }
        private void PrintBuildingOutlines(List<List<(double Lat, double Lon)>> buildingOutlines)
        {
            for (int i = 0; i < buildingOutlines.Count; i++)
            {
                Console.WriteLine($"Building {i + 1} Outline:");
                foreach (var (Lat, Lon) in buildingOutlines[i])
                {
                    Console.WriteLine($"    Latitude: {Lat}, Longitude: {Lon}");
                }
                Console.WriteLine(); // Blank line between buildings
            }
        }

        // Private classes for buildingOutline
        // dotnet's serializer will automatically retain structure of the objects when returning it as JSON
        private class Coordinate
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }
        private class BuildingOutline
        {
            public string BuildingID { get; set; } = string.Empty;
            public List<Coordinate> Coordinates { get; set; } = new List<Coordinate>();
        }
    }
    
}