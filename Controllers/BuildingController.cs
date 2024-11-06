using FuzzySharp;
using Microsoft.AspNetCore.Mvc;
using SimpleWebAppReact.Entities;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
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

        public BuildingController(ILogger<BuildingController> logger, MongoDbService mongoDbService)
        {
            _logger = logger;
            _buildings = mongoDbService.Database?.GetCollection<Building>("building");
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
    }
}