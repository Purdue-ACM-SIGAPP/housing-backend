using Microsoft.AspNetCore.Mvc;
using SimpleWebAppReact.Entities;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using SimpleWebAppReact.Services;

namespace SimpleWebAppReact.Controllers
{
    /// <summary>
    /// Defines endpoints for operations relating the User table
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IMongoCollection<User>? _users;

        public UserController(ILogger<UserController> logger, MongoDbService mongoDbService)
        {
            _logger = logger;
            _users = mongoDbService.Database?.GetCollection<User>("user");
        }

        /// <summary>
        /// gets user, with optional query parameters
        /// </summary>
        /// <param name="name"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="accountType"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<User>> Get([FromQuery] string? name = null, [FromQuery] string? phoneNumber = null, [FromQuery] int? accountType = null)
        {
            // Build the filter using a filter builder
            var filterBuilder = Builders<User>.Filter;
            var filter = FilterDefinition<User>.Empty;

            // Apply the name filter if the parameter is provided
            if (!string.IsNullOrEmpty(name))
            {
                filter &= filterBuilder.Eq(b => b.Name, name);
            }

            // Apply the phoneNumber filter if the parameter is provided
            if (!string.IsNullOrEmpty(phoneNumber))
            {
                filter &= filterBuilder.Eq(b => b.PhoneNumber, phoneNumber);
            }

            // Apply the accountType filter if the parameter is provided,
            // an account type too high or low will be ignored
            if (accountType != null)
            {
                int max = Enum.GetValues(typeof(UserType)).Cast<int>().Max();
                int min = Enum.GetValues(typeof(UserType)).Cast<int>().Min();
                if (accountType >= min || accountType <= max)
                {
                    filter &= filterBuilder.Eq(b => b.AccountType, accountType);
                }
            }

            // Fetch the users from the database using the filter
            return await _users.Find(filter).ToListAsync();
        }

        /// <summary>
        /// gets specific user with id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<User?>> GetById(string id)
        {
            ObjectId objectId = new ObjectId(id);
            // Simple validation to check if the ID is not null
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Invalid ID format.");
            }
            
            var filter = Builders<User>.Filter.Eq(x => x.Id, objectId);
            var user = _users.Find(filter).FirstOrDefault();
            return user is not null ? Ok(user) : NotFound();
        }

        /// <summary>
        /// adds user entry to table
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Post(User user)
        {
            await _users.InsertOneAsync(user);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
            
        }

        /// <summary>
        /// updates a user entry
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<ActionResult> Update(User user)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, user.Id);
            await _users.ReplaceOneAsync(filter, user);
            return Ok();
        }

        /// <summary>
        /// deletes a user entry
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            ObjectId objectId = new ObjectId(id);
            var filter = Builders<User>.Filter.Eq(x => x.Id, objectId);
            await _users.DeleteOneAsync(filter);
            return Ok();
        }
    }
}

