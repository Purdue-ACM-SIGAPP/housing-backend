using Microsoft.AspNetCore.Mvc;
using SimpleWebAppReact.Entities;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using SimpleWebAppReact.Services;
using System.Text.RegularExpressions;

namespace SimpleWebAppReact.Controllers
{
    /// <summary>
    /// Defines endpoints for operations relating the Interaction table
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class InteractionController : ControllerBase
    {
        private readonly ILogger<InteractionController> _logger;
        private readonly IMongoCollection<Interaction>? _interactions;
        private readonly IMongoCollection<User>? _users;
        private readonly IMongoCollection<Review>? _reviews;

        public InteractionController(ILogger<InteractionController> logger, MongoDbService mongoDbService)
        {
            _logger = logger;
            _interactions = mongoDbService.Database?.GetCollection<Interaction>("interaction");
            _users = mongoDbService.Database?.GetCollection<User>("user");
            _reviews = mongoDbService.Database?.GetCollection<Review>("review");
        }

        /// <summary>
        /// gets interactions
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// </summary>
        // [HttpGet("{userId}")]
        // public async Task<ActionResult<Interaction>> GetUserInteractionsById(string userId)
        // {
        //     var interactions = await _interactions.Find(x => x.UserId == userId).ToListAsync();
        //
        //     // Fetches all User-Review entries
        //     return Ok(interactions);
        // }
        
        /// <summary>
        /// gets number of likes on a certain review
        /// </summary>
        /// <param name="reviewId"></param>
        /// <returns></returns>
        /// </summary>
        [HttpGet("{reviewId}")]
        public async Task<ActionResult<Interaction>> GetLikesByReviewId(string reviewId)
        {
            var interactions = await _interactions.Find(x => x.ReviewId == reviewId).ToListAsync();
            
            // Fetches like count of review
            return Ok(interactions.Select(x => (x.Liked == true) ? 1 : 0));
        }

        /// <summary>
        /// adds interaction to the table if and only if user does not have a previous interaction with the review
        /// </summary>
        /// <param name="interaction"></param>
        /// <param name="liked"></param>
        /// <returns></returns>
        [HttpPost("{interaction}")]
        public async Task<ActionResult> Post(Interaction interaction)
        {
            var userId = interaction.UserId;
            var reviewId = interaction.ReviewId;

            // Get user
            var user = await _users.Find(x => x.Id == userId).FirstOrDefaultAsync();

            if (user == null)
            {
                return BadRequest("userId is not valid");
            }
            
            // Get review
            var review = await _reviews.Find(x => x.Id == reviewId).FirstOrDefaultAsync();

            if (review == null)
            {
                return BadRequest("reviewId is not valid");
            }
            
            // Check if interaction between review and user exists already
            var i = await _interactions.Find(x => x.UserId == userId && x.ReviewId == reviewId).FirstOrDefaultAsync();

            if (i != null)
            {
                return BadRequest("User has already interacted with this review!");
            }
            
            // Add interaction
            await _interactions.InsertOneAsync(interaction);

            return Ok();
        }

        /// <summary>
        /// updates an interaction entry
        /// </summary>
        /// <param name="interaction"></param>
        /// <param name="liked"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<ActionResult> Update(Interaction interaction)
        {
            var filter = Builders<Interaction>.Filter.Eq(x => x.Id, interaction.Id);
            await _interactions.ReplaceOneAsync(filter, interaction);
            return Ok();
        }

        /// <summary>
        /// deletes an interaction entry
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var filter = Builders<Interaction>.Filter.Eq(x => x.Id, id);
            await _interactions.DeleteOneAsync(filter);
            return Ok();
        }
    }
}