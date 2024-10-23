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
    /// Defines endpoints for operations relating the Review table
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly ILogger<ReviewController> _logger;
        private readonly IMongoCollection<Review>? _reviews;

        public ReviewController(ILogger<ReviewController> logger, MongoDbService mongoDbService)
        {
            _logger = logger;
            _reviews = mongoDbService.Database?.GetCollection<Review>("review");
        }

        /// <summary>
        /// gets reviews, with optional query parameters
        /// </summary>
        /// <param name="mostRecent"></param>
        /// <returns></returns>
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<Review>> Get([FromQuery] bool mostRecent = true, [FromQuery] string? keywords = null)
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
        public async Task<ActionResult<Review?>> GetById([FromRoute] string id)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            
            var filter = Builders<Review>.Filter.Eq(x => x.Id, id);
            var review = _reviews.Find(filter).FirstOrDefault();
            
            return review is not null ? Ok(review) : NotFound();
        }

        /// <summary>
        /// adds review entry to table
        /// </summary>
        /// <param name="review"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Post(Review review)
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
        public async Task<ActionResult> Update(Review review)
        {
            var filter = Builders<Review>.Filter.Eq(x => x.Id, review.Id);
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
            var filter = Builders<Review>.Filter.Eq(x => x.Id, id);
            await _reviews.DeleteOneAsync(filter);
            return Ok();
        }

        /// <summary>
        /// gets all reviews of the building with the passed buildingId
        /// </summary>
        /// <returns>
        /// a list of all reviews with the passed buildingId
        /// </returns>
        [HttpGet("building/{buildingId}")]
        public async Task<ActionResult> GetByBuildingId([FromRoute] string buildingId)
        {
            // get all reviews with matching buildingId
            var reviews = await _reviews.Find(_ => true).ToListAsync();
            reviews = reviews.Where(x => x.BuildingId == buildingId).ToList();

            // return reviews
            return Ok(reviews);
        }

        /// <summary>
        /// delete all reviews for the building with the passed buildingId
        /// </summary>
        /// <param name="buildingId">he id of the building to delete reviews for</param>
        /// <returns></returns>
        [HttpDelete("building/{buildingId}")]
        public async Task<ActionResult> DeleteByBuildingId([FromRoute] string buildingId)
        {
            // make a filter for all reviews with the passed buildingId and remove filter from database
            var filter = Builders<Review>.Filter.Eq(x => x.BuildingId, buildingId);
            await _reviews.DeleteManyAsync(filter);
            return Ok();
        }

        /// <summary>
        /// gets the average rating of the building with the passed buildingId
        /// </summary>
        /// <param name="buildingId">the id of the building to get the average rating for</param>
        /// <returns>
        /// the average rating of the passed buildingId
        /// or -1 if no ratings exist
        /// </returns>
        [HttpGet("building/average/{buildingId}")]
        public async Task<ActionResult> GetAverageRatingForBuilding([FromRoute] string buildingId)
        {
            // get all reviews with matching buildingId
            var reviews = await _reviews.Find(_ => true).ToListAsync();
            reviews = reviews.Where(x => x.BuildingId == buildingId).ToList();

            // get the average rating
            var ratings = reviews.Select(x => x.Rating).ToList();
            var sum = ratings.Sum();
            var num = ratings.Capacity;
            var average = (num == 0) ? -1 : Convert.ToDouble(sum) / num;

            // return average
            return Ok(average);
        }
    }
}