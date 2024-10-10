using Microsoft.AspNetCore.Mvc;
using SimpleWebAppReact.Entities;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using SimpleWebAppReact.Services;

namespace SimpleWebAppReact.Controllers
{
    /// <summary>
    /// Defines endpoints for operations relating the VideoTour table
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class VideoTourController : ControllerBase
    {
        private readonly ILogger<VideoTourController> _logger;
        private readonly IMongoCollection<VideoTour>? _videoTours;

        public VideoTourController(ILogger<VideoTourController> logger, MongoDbService mongoDbService)
        {
            _logger = logger;
            _videoTours = mongoDbService.Database?.GetCollection<VideoTour>("videoTour");
        }

        /// <summary>
        /// gets videoTours
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<VideoTour>> Get()
        {
            // Fetch the videoTours from the database
            var videoTours = await _videoTours.Find(_ => true).ToListAsync();

            return Ok(videoTours);
        }

        /// <summary>
        /// gets specific videoTour with id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<VideoTour?>> GetById(string id)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            
            var filter = Builders<VideoTour>.Filter.Eq(x => x.Id, id);
            var videoTour = _videoTours.Find(filter).FirstOrDefault();
            
            return videoTour is not null ? Ok(videoTour) : NotFound();
        }

        /// <summary>
        /// adds videoTour entry to table
        /// </summary>
        /// <param name="videoTour"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Post(VideoTour videoTour)
        {
            await _videoTours.InsertOneAsync(videoTour);
            return CreatedAtAction(nameof(GetById), new { id = videoTour.Id }, videoTour);
        }

        /// <summary>
        /// updates a videoTour entry
        /// </summary>
        /// <param name="videoTour"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<ActionResult> Update(VideoTour videoTour)
        {
            var filter = Builders<VideoTour>.Filter.Eq(x => x.Id, videoTour.Id);
            await _videoTours.ReplaceOneAsync(filter, videoTour);
            return Ok();
        }

        /// <summary>
        /// deletes a videoTour entry
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var filter = Builders<VideoTour>.Filter.Eq(x => x.Id, id);
            await _videoTours.DeleteOneAsync(filter);
            return Ok();
        }
    }
}

