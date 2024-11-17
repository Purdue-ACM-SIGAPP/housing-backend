namespace SimpleWebAppReact.Controllers;
using Microsoft.AspNetCore.Mvc;
using SimpleWebAppReact.Services;
using SimpleWebAppReact.Entities;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

[ApiController]
[Route("api/maps")]
public class MapsController : ControllerBase
{
    private readonly ILogger<MapsController> _logger;
    private readonly IMongoCollection<Building>? _buildings;
    private readonly HttpClient _httpClient;
    private readonly string _googleApiKey = "AIzaSyCzKs4kUhXuPhBxYB2BU0ODXXIUBJnenhA";

    public MapsController(ILogger<MapsController> logger, MongoDbService mongoDbService, HttpClient httpClient)
    {
        _logger = logger;
        _buildings = mongoDbService.Database?.GetCollection<Building>("building");
        _httpClient = httpClient;
    }
    
    [HttpGet("distance")]
    public async Task<IActionResult> GetDistance(string buildingId1, string buildingId2)
    {
        if (_buildings == null)
        {
            return StatusCode(500, new { message = "Building collection not initialized." });
        }

        // Fetch the two buildings from the database using their IDs
        var building1 = await _buildings.Find(b => b.Id == buildingId1).FirstOrDefaultAsync();
        var building2 = await _buildings.Find(b => b.Id == buildingId2).FirstOrDefaultAsync();

        if (building1 == null || building2 == null)
        {
            return NotFound(new { message = "One or both buildings not found." });
        }

        // Extract addresses
        string address1 = building1.Address ?? string.Empty;
        string address2 = building2.Address ?? string.Empty;

        if (string.IsNullOrEmpty(address1) || string.IsNullOrEmpty(address2))
        {
            return BadRequest(new { message = "Addresses are missing for one or both buildings." });
        }

        // Prepare the Distance Matrix API request
        string url = $"https://maps.googleapis.com/maps/api/distancematrix/json?origins={address1}&destinations={address2}&units=imperial&key={_googleApiKey}";

        try
        {
            // Send the request to Google Maps API
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var jsonData = JObject.Parse(jsonResponse);

            // Parse distance and duration from the response
            var distance = jsonData["rows"]?[0]?["elements"]?[0]?["distance"]?["text"]?.ToString();
            var duration = jsonData["rows"]?[0]?["elements"]?[0]?["duration"]?["text"]?.ToString();

            if (distance == null || duration == null)
            {
                return BadRequest(new { message = "Could not calculate distance or duration." });
            }

            // Return the distance and duration as the response
            return Ok(new
            {
                building1 = building1.Name,
                building2 = building2.Name,
                distance,
                duration
            });
        }
        catch (HttpRequestException e)
        {
            _logger.LogError(e, "Error while calling Google Maps API");
            return StatusCode(500, new { message = "Error while calling Google Maps API." });
        }
    }
    [HttpPost("set-coordinates")]
    public async Task<IActionResult> SetCoordinates(string buildingId)
    {
        if (_buildings == null)
        {
            return StatusCode(500, new { message = "Building collection not initialized." });
        }

        // Fetch the building from the database using its ID
        var building = await _buildings.Find(b => b.Id == buildingId).FirstOrDefaultAsync();

        if (building == null)
        {
            return NotFound(new { message = "Building not found." });
        }

        // Ensure the address is not null or empty
        if (string.IsNullOrEmpty(building.Address))
        {
            return BadRequest(new { message = "Address is missing for the building." });
        }

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

            // Update the building in the database
            var updateResult = await _buildings.ReplaceOneAsync(b => b.Id == buildingId, building);

            if (!updateResult.IsAcknowledged)
            {
                return StatusCode(500, new { message = "Failed to update building coordinates in the database." });
            }

            return Ok(new
            {
                message = "Coordinates updated successfully.",
                buildingId,
                latitude,
                longitude
            });
        }
        catch (HttpRequestException e)
        {
            _logger.LogError(e, "Error while calling Google Maps API");
            return StatusCode(500, new { message = "Error while calling Google Maps API." });
        }
    }

}

