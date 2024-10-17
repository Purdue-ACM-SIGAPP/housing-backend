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
    /// Defines endpoints for operations relating the UserReview table
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UserReviewController : ControllerBase
    {
        private readonly ILogger<UserReviewController> _logger;
        private readonly IMongoCollection<UserReview>? _reviews;

        public UserReviewController(ILogger<UserReviewController> logger, MongoDbService mongoDbService)
        {
            _logger = logger;
            _reviews = mongoDbService.Database?.GetCollection<UserReview>("userReview");
        }

        /// <summary>
        /// gets reviews, with optional query parameters
        /// </summary>
        /// <param name="mostRecent"></param>
        /// <returns></returns>
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<UserReview>> Get(User user)
        {
            var reviews = await _reviews.Find(_ => true).ToListAsync();
            
            if (!string.IsNullOrWhiteSpace(keywords))
            {
                // Split keywords into individual words by space (you can change the delimiter if needed)
                var keywordArray = keywords.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        
                // Filter reviews by checking if the description contains any of the keywords
                reviews = reviews.Where(x => keywordArray.Any(kw => 
                    Regex.IsMatch(x.Description, $".*{kw}.*", RegexOptions.IgnoreCase)
                )).ToList();
            }

            if (mostRecent)
            {
                reviews.Reverse();
            }

            // Fetch the reviews from the database
            return Ok(reviews);
        }

        /// <summary>
        /// gets specific review with id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<UserReview?>> GetById([FromRoute] string id)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            
            var filter = Builders<UserReview>.Filter.Eq(x => x.Id, id);
            var review = _reviews.Find(filter).FirstOrDefault();
            
            return review is not null ? Ok(review) : NotFound();
        }

        /// <summary>
        /// adds review entry to table
        /// </summary>
        /// <param name="review"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Post(UserReview review)
        {
            await _reviews.InsertOneAsync(review);
            return CreatedAtAction(nameof(GetById), new { id = review.Id }, review);
        }

        /// <summary>
        /// updates a review entry
        /// </summary>
        /// <param name="review"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<ActionResult> Update(UserReview review)
        {
            var filter = Builders<UserReview>.Filter.Eq(x => x.Id, review.Id);
            await _reviews.ReplaceOneAsync(filter, review);
            return Ok();
        }

        /// <summary>
        /// deletes a review entry
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var filter = Builders<UserReview>.Filter.Eq(x => x.Id, id);
            await _reviews.DeleteOneAsync(filter);
            return Ok();
        }
    }
}