using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

public class BuildingOutlineService
{
    private readonly HttpClient _httpClient;

    public BuildingOutlineService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Method to get building outlines with multiple coordinates for diverse polygon shapes
    public async Task<List<List<(double Lat, double Lon)>>> GetBuildingOutline(double minLat, double minLon, double maxLat, double maxLon)
    {
        // Overpass API endpoint
        string url = "http://overpass-api.de/api/interpreter";

        // Create Overpass QL query for buildings within the bounding box
        string query = $@"
            [out:json];
            (
              way[""building""]({minLat}, {minLon}, {maxLat}, {maxLon});
            );
            out geom;
        ";

        // Send the query as a POST request
        var response = await _httpClient.PostAsync(url, new StringContent(query));
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();

        // Parse JSON response to extract coordinates for polygons
        var buildingPolygons = ParseBuildingCoordinates(json);

        return buildingPolygons;
    }

    // Helper method to parse JSON and generate list of coordinates for diverse polygon shapes
    private List<List<(double Lat, double Lon)>> ParseBuildingCoordinates(string json)
    {
        var data = JsonDocument.Parse(json);
        var buildings = new List<List<(double Lat, double Lon)>>();

        // Loop through each element to find building polygons with diverse shapes
        foreach (var element in data.RootElement.GetProperty("elements").EnumerateArray())
        {
            if (element.GetProperty("type").GetString() == "way" && element.TryGetProperty("geometry", out var geometry))
            {
                var coordinates = new List<(double Lat, double Lon)>();

                // Collect each point's latitude and longitude for this building polygon
                foreach (var point in geometry.EnumerateArray())
                {
                    double lat = point.GetProperty("lat").GetDouble();
                    double lon = point.GetProperty("lon").GetDouble();
                    coordinates.Add((lat, lon));
                }

                // Ensure the polygon is closed by adding the first coordinate at the end if necessary
                if (coordinates.Count > 0 && coordinates[0] != coordinates[^1])
                {
                    coordinates.Add(coordinates[0]);
                }

                buildings.Add(coordinates);
            }
        }

        return buildings;
    }
}
