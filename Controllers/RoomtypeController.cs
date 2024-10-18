using Microsoft.AspNetCore.Mvc;
using SimpleWebAppReact.Entities;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using SimpleWebAppReact.Services;

namespace SimpleWebAppReact.Controllers
{
    /// <summary>
    /// Defines endpoints for operations relating the Building table
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class RoomtypeController : ControllerBase
    {
        private readonly ILogger<BuildingController> _logger;
        private readonly IMongoCollection<RoomType>? _buildings;

        public RoomtypeController(ILogger<BuildingController> logger, MongoDbService mongoDbService)
        {
            _logger = logger;
            _buildings = mongoDbService.Database?.GetCollection<RoomType>("RoomType");
        }

        /// <summary>
        /// gets buildings, with optional query parameters
        /// </summary>
        /// <param name="id"></param>
        /// <param name="buildingId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<RoomType>> Get([FromQuery] string[]? images, 
            [FromQuery] int? peoplePerBedroom, [FromQuery] string? description,[FromQuery] double? cost,
            [FromQuery] ObjectId? buildingID)
        {
            // Build the filter using a filter builder
            var filterBuilder = Builders<RoomType>.Filter;
            var filter = FilterDefinition<RoomType>.Empty;
            //TODO: make the images array work
            if (!string.IsNullOrEmpty(images))
            {
                filter &= filterBuilder.Eq(b => b.Images, images);
            }
            if (!peoplePerBedroom.HasValue)
            {
                filter &= filterBuilder.Eq(b => b.PeoplePerBedroom, peoplePerBedroom);
            }
            // Apply the name filter if the parameter is provided
            if (!string.IsNullOrEmpty(description))
            {
                filter &= filterBuilder.Eq(b => b.Description, description);
            }
            if (!cost.HasValue)
            {
                filter &= filterBuilder.Eq(b => b.Cost, cost);
            }
            // Apply the address filter if the parameter is provided
            //TODO: still need to do
            if (!ObjectId.(buildingID))
            {
                filter &= filterBuilder.Eq(b => b.BuildingID, buildingID);
            }

            // Fetch the buildings from the database using the filter
            return await _buildings.Find(filter).ToListAsync();
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


