using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleWebAppReact.Entities;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using SimpleWebAppReact.Services;

namespace SimpleWebAppReact.Controllers
{
    /// <summary>
    /// Defines endpoints for operations relating the Events table
    /// </summary>
    [ApiController]
    [Route("api/events")]
    public class EventsController : ControllerBase
    {
        private readonly ILogger<EventsController> _logger;
        private readonly IMongoCollection<Events>? _events;

        public EventsController(ILogger<EventsController> logger, MongoDbService mongoDbService)
        {
            _logger = logger;
            _events = mongoDbService.Database?.GetCollection<Events>("events");
        }

        /// <summary>
        /// gets events, with optional query parameters
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<Events>> Get([FromQuery] string? eventName = null, [FromQuery] string? summary = null, [FromQuery] string? content = null, [FromQuery] string? userID = null, [FromQuery] DateTime? date = null, [FromQuery] string? address = null)
        {
            // Build the filter using a filter builder
            var filterBuilder = Builders<Events>.Filter;
            var filter = FilterDefinition<Events>.Empty;

            // Apply the event name filter if the parameter is provided
            if (!string.IsNullOrEmpty(eventName))
            {
                filter &= filterBuilder.Eq(b => b.EventName, eventName);
            }

            // Apply the summary filter if the parameter is provided
            if (!string.IsNullOrEmpty(summary))
            {
                filter &= filterBuilder.Eq(b => b.Summary, summary);
            }

            if (!string.IsNullOrEmpty(content))
            {
                filter &= filterBuilder.Eq(b => b.Content, content);
            }

            if (!string.IsNullOrEmpty(userID))
            {
                filter &= filterBuilder.Eq(b => b.UserID, userID);
            }

            if (!date.HasValue)
            {
                filter &= filterBuilder.Eq(b => b.Date, date);
            }

            if (!string.IsNullOrEmpty(address))
            {
                filter &= filterBuilder.Eq(b => b.Address, address);
            }

            // Fetch the events from the database using the filter
            return await _events.Find(filter).ToListAsync();
        }

        /// <summary>
        /// gets specific events with id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        //deleted asyn
        public async Task<ActionResult<Events?>> GetById(string id)
        {
            // Simple validation to check if the ID is not null
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Invalid ID format.");
            }
            
            var filter = Builders<Events>.Filter.Eq(x => x.Id, id);
            var events = _events.Find(filter).FirstOrDefault();
            return events is not null ? Ok(events) : NotFound();
        }

        /// <summary>
        /// adds events entry to table
        /// </summary>
        /// <param name="events"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Student,ResidentAssistant,GreekLife,Admin")]
        public async Task<ActionResult> Post(Events events)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized(new { Message = "User ID not found in claims." });
            }

            events.UserID = userId;
            
            await _events.InsertOneAsync(events);
            return CreatedAtAction(nameof(GetById), new { id = events.Id }, events);
            
        }

        /// <summary>
        /// updates a events entry
        /// </summary>
        /// <param name="events"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Roles = "Student,ResidentAssistant,GreekLife,Admin")]
        public async Task<ActionResult> Update(Events events)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized(new { Message = "User ID not found in claims." });
            }

            var filter = Builders<Events>.Filter.Eq(x => x.Id, events.Id);
            
            var roles = User.Claims
                .Where(c => c.Type == ClaimTypes.Role) // Use ClaimTypes.Role to fetch role claims
                .Select(c => c.Value)
                .ToList();
            
            var evt = await _events.Find(filter).FirstOrDefaultAsync();

            if (!roles.Contains(UserType.Admin.ToString()) && !evt.UserID.Equals(userId))
            {
                return Unauthorized();
            }
            
            await _events.ReplaceOneAsync(filter, events);
            return Ok();
        }

        /// <summary>
        /// deletes a events entry
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Student,ResidentAssistant,GreekLife,Admin")]
        public async Task<ActionResult> Delete(string id)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized(new { Message = "User ID not found in claims." });
            }

            var filter = Builders<Events>.Filter.Eq(x => x.Id, id);
            
            var roles = User.Claims
                .Where(c => c.Type == ClaimTypes.Role) // Use ClaimTypes.Role to fetch role claims
                .Select(c => c.Value)
                .ToList();
            
            var evt = await _events.Find(filter).FirstOrDefaultAsync();

            if (!roles.Contains(UserType.Admin.ToString()) && !evt.UserID.Equals(userId))
            {
                return Unauthorized();
            }
            
            await _events.DeleteOneAsync(filter);
            return Ok();
        }
    }
}

